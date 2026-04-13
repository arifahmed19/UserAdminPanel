🚀 UserAdminPanel: My User Management Solution
Hey there! I'm Arif Ahmed. This is my implementation of a full-stack User Management System. I built this to handle the messy parts of user administration—blocking, deleting, and tracking activity—while keeping the interface sleek and modern.

🎨 The Vibe
I didn't want this to look like a boring internal tool. I took inspiration from the Lotus Team design for the login experience.

It’s built with C#, ASP.NET Core MVC, and PostgreSQL (Supabase).

🛠️ What I Built (And why it’s cool)
🔒 Security & Access
The "Kicking People Out" Logic: I wrote a custom Middleware that checks your status on every single click. If an admin blocks or deletes you while you're still browsing, the next thing you'll see is the login screen. No "soft" sessions here.

1-Character Passwords: Per the requirements, I kept the password rules wide open. If you want your password to be 1, go for it.

The Vault: Every user has a Unique Index on their email in the database. Even if the frontend fails, the Supabase layer won't allow duplicate accounts.

📊 The Admin Dashboard
Bulk Actions: You can select a bunch of users (or everyone) and Block, Unblock, or Delete them all at once.

Real-time Activity: The table automatically sorts by the Last Login Time, so you always see who was active most recently at the top.

Actually Deleted: When you hit delete, the user is hard-deleted from the database. No "archived" flags, just gone.

📧 Async Onboarding
When a new user registers, they are saved to the database immediately. I set up the email verification to run as an asynchronous background task, so the user doesn't have to sit there watching a loading spinner while the email sends.

🏗️ The Tech Stack
Framework: ASP.NET Core 10.0 (MVC)

Database: PostgreSQL (Hosted on Supabase Sydney)

Frontend: Bootstrap 5 + Bootstrap Icons

Deployment: Render (using Docker)

🚀 Running it yourself
If you want to poke around the code:

Clone it:
git clone https://github.com/arifahmed19/UserAdminPanel.git

Database:
Grab a PostgreSQL connection string (Supabase works great). Toss it into appsettings.json.

Migrate:
dotnet ef database update

Go:
dotnet run

📝 Final Thoughts
The trickiest part of this project wasn't the code—it was the deployment! Getting Render (IPv4) to talk to Supabase Sydney (IPv6) required some networking gymnastics with connection pooling and disabling IPv6 at the system level.

Feel free to reach out if you have questions!

Arif Ahmed | GitHub
