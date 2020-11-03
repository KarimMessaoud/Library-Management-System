# Library-Management-System

Built in ASP.NET Core 3.1 and C#. </br>
Database Management System: SQL Server 2019.

<img width="957" alt="Screenshot" src="https://user-images.githubusercontent.com/50749737/96971823-f55de500-1515-11eb-847e-9d274f7b2554.png">

<h2>Description</h2>
An integrated library system to lend and manage library assets. The software helps to control all the library items, for example,
what patrons have what assets, what patrons have certain items on hold.
It also helps to track the different branches that belong to the library. 
Various people might sign up for a library card within the system. The application allows to store necessary information of all users
and charge their fees as well. 
The system  automatically checks whether the patrons who have placed hold on some library assets have checked them out in the set time limit
or not and then sends them emails with the respective information.
The application has also automatic email confirmation to members on successful registration
and automatic email sending when resetting password by users. </br>
This project was inspired by Wes Doyle ASP.NET Core Web App Tutorial on YouTube which was also very helpful to build this application at the beginnig of my work.

<h6>The Project have 4 types of users:</h6>
<ol type="1">
	<li>Common user</li>
	<li>Library user (Patron)</li>
	<li>Employee</li>
	<li>Admin</li>
</ol>

According to their role, they have different tasks and rights.

<h5>Common user</h5>
<ul>
	<li>Search books.</li>
	<li>Register.</li>
	<li>View branches.</li>
</ul>
    
    
    
    
<h5>Patron</h5>
<ul>
	<li>Log in.</li>
	<li>Search books or videos by title or author.</li>
	<li>Request for books.</li>
	<li>View and edit profile.</li>
	<li>Charge overdue fees.</li>
	<li>Change password.</li>
</ul>
    
<h5>Employee</h5>
<ul>
	<li>All Patron's rights and the following:</li>
	<li>Add new books or videos.</li>
	<li>Edit and delete books or videos.</li>
	<li>Place hold on a book or a video.</li>
	<li>Issue requested books.</li>
	<li>Check in a returned book.</li>
	<li>Search patrons by last name.</li>
	<li>Add new patrons.</li>
	<li>View, edit and delete patrons.</li>
	<li>View the list of all patrons.</li>
	<li>Charge and reset overdue fees.</li>
	<li>View branches.</li>
	<li>Add new employee.</li>
</ul>
   
<h5>Admin</h5>
<ul>
	<li>All Employee's rights and the following:</li>
	<li>View the list of all users.</li>
	<li>Add new employee.</li>
	<li>View, edit and delete employee.</li>
	<li>Assign roles to users.</li>
	<li>Add new roles.</li>
	<li>Edit roles.</li>
	<li>Add or remove users from the given role.</li>
	<li>Add claims to the user.</li>
</ul>
    
<h2>Getting Started</h2>

    Download the project
    Open project with Visual Studio and wait for dependencies to be resolved
    Set Library as Startup project
    Open Package Manager Console and make sure that Default project is LibraryData
    Run the following command: update-database
    Populate database with demo data:
        - Go to https://github.com/KarimMessaoud/Demo-Data-Scripts/blob/main/Library_DemoData.sql
        - Copy the contents of Library_DemoData.sql file
        - Open SQL Server Management Studio and connect to server
        - Click New Query on the top bar, paste data which was copied and click Execute (below the New Query button)

<h6>After running application:</h6>
	<ul>
		<li>Sign in as Admin, Employee or Patron using the below fictional users.</li>
	</ul>

<h6>If you want to see how emails are sending:</h6>
<ul>
	<li>
		Go to appsettings.json file in the Library project and in the EmailConfiguration section put your data as follows:</br>
    			"SenderEmail": "your gmail address",</br>
    			"Password": "password to your gmail account"
	</li>
	<li>
	    Create new user giving him your own email address:
      <ul>
        <li>Log in as Admin</li>
        <li>In the top navigation bar click Manage -> Users -> Add new patron or Add new employee</li>
      </ul>
	</li>
</ul>

<b>Admin</b></br>
email: admin@oetl.pl</br>
password: Demo123$

<b>Employee</b></br>
email: employee@oetl.pl</br>
password: Demo123$
