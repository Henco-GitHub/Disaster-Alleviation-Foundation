# Disaster Alleviation Foundation

Web application built in Visual Studio 2022, using ASP.NET MVC 5. The application allows users to register and donate money and goods to the organisation, as well as
record disasters to which donations can be allocated to - an admin user is able to allocate all donations to certain disasters of choice, based on the information 
gathered, eg. via description, goods needed, etc. 

The admin user can either accept or reject a new (pending) user after user registration. The admin user can use donated money to buy goods that can be donated. 
The admin user can view all goods donated, as well as records on cash flow in the organisation. 

## User login details to use web app:
### Admin
- Email: admin@gmail.com
- Password: admin

### Regular user (that is approved by admin and can access site)
- Email: user@gmail.com
- Password: useruser

## Features
### User

- User login and registration
- View public page
- User can donate money
- User can donate goods
- User can record a new active disaster
- View user monetary donations
- View user goods donations
- View user recorded disasters
- View community monetary donations
- View community goods donations
- View community recorded disasters

### Admin

- View public page
- Accept or reject pending users
- View active disasters
- Allocate money to a disaster
- Allocate goods to a disaster
- Purchase goods using available money from donations
- View information on goods 
- View cash flow 

## changes made from Part 2
-Added currency formatting to all amounts

-On-load page changed from Login to Public page

-Available money is displayed to admin user on modal when allocating money to a disaster

-Available items is displayed to admin user on modal when allocating goods to a disaster

-Changed partial layouts to accompany the three different users in the system (change nav bar items)


## changes made from Part 1
-Ensured that all number inputs cannot be 0 or less (Monetary and goods donations - from task 1)

-Changed the order of the input fields in the goodsDonations form (Category before number of items to donate)

-User no longer need to type their name in when donating money or goods - the user simply has to tick a checkbox to donate anonymously or not

-Updated nav bar (now using partial layouts to distingiush between normal registered user and admin user)

-added icons to whole webapp

-changed button placements

-changed button colors

-changed logo

## Part 3 - 11/11/2022
-public accesseble page with data regarding monetary and goods donations, as well as disasters and the allocated information respectively

-added unit tests for users

-added unit tests for all logic

-added unit tests for models


## Part 2 - 17/10/2022
-authorised users can allocate money to an active disaster

-authorised users can allocate goods to an active disaster

-authorised users can purchase new goods with available money



## Part 1 - 14/9/2022
-login and register

-purple and orange colour scheme

-authorised users must be able to donate money

-authorised users must be able to donate goods

-donations can be anonymous

-goods donations (pre-configured categories(clothes and non-perishable foods))

-authorised users can define new categories of goods

-authorised users can capture a new disaster

-authorised users can view lists of all monetary and goods donations, as well as all disasters recorded


