# Blogs

A simple blog application created via C# Windows Forms, connected to postgres database in pgAdmin4.

You can create your own account and before the first login, one of the admins has to approve your registration. Afterwards, you can write your own post about whatever you want (also being able to edit something if you typed it wrong or whatever) and other users can see your posts and make comments on it. Your posts are only visible on the "board" if your account status is Active, which, once again, is managed by admin. Owners of the posts have the ability to delete comments on their post if they don't like it or what ever other reason.

Short steps on how to run the code:

1. Download Visual studio + npgsql nugget package
2. Download pgAdmin4, create server and database with settings/names mentioned in ConnectingToDatabase.cs class (localhost, 5432, postgres, Blog)
3. Import the script from Database folder and run it

Every method and click events you can find in the code are described with a comment.
