# Application Development Examples
This repository provides example portfolio example development in C#, Blazor, etc.

![Example Application process](https://github.com/makalkas/Portfolio_Development_Examples/blob/main/Images/2024-12-11%2018_42_56-Application%20Processes.vsdx%20-%20Visio%20Professional.png)
Most applications will generally follow a simple data flow process/method. 
The user clicks a button, the application should respond by accepting the user event. Some process kicks off from a timer/scheduler, etc. (Extract)
The operation should then transform some kind of data, wether that be a mouse location, a call to open another window, accept 
some kind of user input, move data, etc. this must transformed into some kind of useable data or work (Transform).
This transformed data must then be moved to different endpoints. Usually one of the endpoints is some kind of a graphic on the
screen. Another common endpoint is to a database, file/directory, or service. (Load)

## Example Blazor Architecture
![Blazor Architecture](https://github.com/makalkas/Portfolio_Development_Examples/blob/main/Images/Blazor%20Enterprise%20App%20Architechture.png)

The above Application process architecture is then expanded into what you see in the Blazor Architecture example (directly above) to allow for
efficient communication and processing. The different services allow the application to grow and change as the business does while reducing the
amount of work needed to upgrade the application.

# UI/UX

![Example Basic UI](https://github.com/makalkas/Portfolio_Development_Examples/blob/main/Images/UI-LabelApplication.png)

When building UI(s), getting user feed back is important. For this reason, I try to give the user a version that is devoid of any destractions until
I am ready to add the elements that will help make everything as intuitive as possible. This means following the patterns and conventions of applications
the user is familiar with. This is why showing the user the UI as early in development as possible is critical.

# C#

There are several things that are important to me as a developer when building anything with C#. First proper application of the "SOLID", "DRY", "OOP", and "clean code" principals.
Second, following the standard coding conventions document that most companies have. Visual Studio makes most of this easy and has a lot of it already built in. However, some
developers seem to always fight the IDE's suggestions and want to do it their way. This is almost never good. There are several good examples out on the web that new developers
should take a look at ([Example Coding Standards Doc](https://www.bing.com/ck/a?!&&p=8dc1353254b73e4bd13ddc75ec7fe7d1934baafae699e9a1a519b70c645e9ba9JmltdHM9MTczMzk2MTYwMA&ptn=3&ver=2&hsh=4&fclid=0cdd8e75-1f03-6755-337e-9a171e2f6605&psq=Example+C%23+coding+standards+document&u=a1aHR0cHM6Ly9hc3BibG9ncy5ibG9iLmNvcmUud2luZG93cy5uZXQvbWVkaWEvbGh1bnQvUHVibGljYXRpb25zL0NTaGFycCUyMENvZGluZyUyMFN0YW5kYXJkcy5wZGY&ntb=1)).
([Desktop App. Example](https://github.com/makalkas/MyCustomCalculator)) 

# ASP.NET

# Blazor

# HTML

# JavaScript


