
************ Building a REST API ********************
-----------------------------------------------------

Operating System: Windows 10

Name of web application: PowerplantCodingChallengeAPI


- Building of application
I built the application with Visual Studio 2019. It is a web API ASP.NET Core (.NET 5.0) project.
The project is separated in 3 layers:
	- User Interface Layer (PowerplantCodingChallengeAPI) -> It contains the RESTFul HTTP  service.
	- Business Logic Layer (BLL) -> It is a class library type project which is responsible for managing business roles.
	- Data Access Layer (DAL) -> It is a class library type project which is responsible for access data. It lets us to read and write in a JSON file.

	- NuGet packages used:
		- Microsoft.AspNetCore.Http.Features 5.0.13 version --> ASP.NET Core HTTP feature interface definitions
		- Newtonsoft.Json 13.0.1 version --> JSON framework for .NET (To serialize and deserialize data)
		-  Swashbuckle.AspNetCore 6.2.3 version --> Swagger tools for documenting APIs built on ASP.NET Core 

- Launch the API
	- First of all insure that you have Visual Studio 2019 installed on you computer.
	- You can open the project from Github repository with Visual Studio 2019 or download the project from Github repository on your computer then
	  unzip and run "PowerplantCodingChallengeAPI.sln".
	- In Visual Studio 2019 from the top menu (green start button) insure that PowerplantCodingChallengeAPI is selected.
	- To run the program, press Ctrl + F5 or select Debug > Release from the top menu then click on the green start button.
	- The application runs the swagger tool on your browser by default, allowing you to test the API.
	- Download on your computer example_payload(three JSON files) from Github (https://github.com/gem-spaas/powerplant-coding-challenge)
	- Now on "Payload" section (endpoint: https://localhost:8888/api/Payload), click on "POST button > Try it out", you can upload one of three JSON files then click on "Execute button" to post a payload.
	- On "Productionplan" section (endpoint: https://localhost:8888/api/Productionplan), click on "GET button > Try it out" then click on "Execute button". The WebAPI returns a production plan based on the received payload.
	- Enjoy!

- Notes: in the DAL project, we have a service which deals with errors. Errors are recorded in the "log.dat" file. The date, hour and the type of error are saved.

Thanks!
