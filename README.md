# Hospital Simulator Service #

A Web Service that simulates a hospital where patients will be registered and based on their condition a consultation will be scheduled with a doctor. This solution is developed using .NET Framework 4.6.1 and developed in Visual Studio 2017.
I'm using OWIN in this project to self-host the Web API web service. 


## How To Build ##

This project is developed in Visual Studio 2017 and requires .NET Framework 4.6.1. Once the project is download from GitHub, open the solution HospitalSimulatorService.sln
Build the solution. The following NuGet packages have been used in this project:
- Microsoft.AspNet.WebApi
- Microsoft.AspNet.WebApi.Cors
- Microsoft.Owin
- Microsoft.AspNet.WebApi.Owin
- Microsoft.Owin.Hosting
- Microsoft.Owin.Host.HttpListener
- Newtonsoft.json

After a successful build, HospitalSimulatorService.exe will be generated along with all the required binaries. User can run the server from Visual Studio or run HospitalSimulatorService.exe. Running the web service will open a command console. Clicking any key will terminate the web service session. Service will be running as long as the command line is open.
When application runs, it will populate resrouce objects from provided file "seed.json". Data is only persisted during that session and it is not stored in any database.

The REST APIs have been tested by Restlet Client (Chrome extension) as well as HospitalSimulatorClient.exe application which is developed for accessing this web service. 


## Available APIs ##

#### Register Patient ####

**URL:** /patients
**Method:** POST
**Sample JSON for test using Restlet Client**
```
{
	"name": "Test One",
	"condition": {
		"Type": "Cancer",
		"Topology": "Head&Neck"
	}
}
```
**Successful POST Response:** Code 201 - Created

#### Get All Patients ####

**URL:** /patients
**Method:** GET
**Sample Result JSON**
```
[
	{
		"Name": "Test One",
		"Condition": {
			"Type": "Cancer",
			"Topology": "Head&Neck"
		}
	},
	{
		"Name": "Test Two",
		"Condition": {
			"Type": "Flu",
			"Topology": ""
		}
	},
	{
		"Name": "Test Three",
		"Condition": {
			"Type": "Cancer",
			"Topology": "Breast"
		}
	}
]
```
**Successful GET Response:** Code 200 - OK

#### Get All Consultations ####

**URL:** /consultations
**Method:** GET
**Sample Result JSON**
```
[
	{
		"Patient": {
			"Name": "Test One",
			"Condition": {
				"Type": "Cancer",
				"Topology": "Head&Neck"
			}
		},
		"Doctor": {
			"Name": "John",
			"Roles": [ "Oncologist" ]
		},
		"TreatmentRoom": {
			"Name": "One",
			"TreatmentMachine": {
				"Name": "Elekta",
				"Capability": null
			}
		},
		"RegistrationDate": "2017-12-13T00:00:00-08:00",
		"ConsultationDate": "2017-12-14T00:00:00"
	},
	{
		"Patient": {
			"Name": "Test Two",
			"Condition": {
				"Type": "Flu",
				"Topology": ""}
			},
		"Doctor": {
			"Name": "Anna",
			"Roles": [ "GeneralPractitioner" ]
		},
		"TreatmentRoom": {
			"Name": "Four",
			"TreatmentMachine": {
				"Name": "",
				"Capability": null
			}
		},
		"RegistrationDate": "2017-12-13T00:00:00-08:00",
		"ConsultationDate": "2017-12-14T00:00:00"
	},
	{
		"Patient": {
			"Name": "Test Three",
			"Condition" :{
				"Type": "Cancer",
				"Topology": "Breast"}
			},
		"Doctor": {
			"Name": "Peter",
			"Roles": [ "Oncologist", "GeneralPractitioner"]
		},
		"TreatmentRoom": {
			"Name": "Three",
			"TreatmentMachine": {
				"Name": "MM50",
				"Capability": null
			}
		},
		"RegistrationDate": "2017-12-13T00:00:00-08:00",
		"ConsultationDate": "2017-12-14T00:00:00"
	}
]
```

**Successful GET Response:** Code 200 - OK

## REST API implementation ##

This REST service is implemented using WebApi and hosted on OWIN. This Web API will be available on port 5000 (http://localhost:5000/)
the base URL value is stored in resource file of the project.
There are two controllers, PatientsController is used for registering patients (POST) as well as getting list of all the patients (GET)
ConsultationsController is used to get all the scheduled consultations (GET)


## Consultation Scheduling Logic ##

The logic for scheduling consultation on the next available day is implemented in "ConsultationScheduler" class.
In order to schedule a consultation on the next available day, first the maximum possbile consulations per day is calculated. Since every consulation requires a doctor and a Treatment Room, the maximum number of possible consulation would be the lesser count of number of Doctors and number of Treatment Rooms.
For every future day, get the count for scheduled consulations and if it's less than the maximum possbile consulations, then the patient condition is checked. If patient condition is "Flu" then a check is done to find any available GeneralPractitioner for that day and also a Treatment Room. For Flu patients, first check would be to find any available Treatment Room that doesn't have a Treatment Machine. Otherwise a check is done for Treatment Rooms with Treatment Machine.
If the patient condition is "Cancer", the search for available Doctors with "Oncology" role. Depending on the Topology, search for available Treatment Rooms that have Treatment Machine that matches the patient's Topology.
If found, the consultation will be scheduled. If not found, go advance the day and perform the check again.


## Unit Tests ##

There are two unit test implemented to test Patients controller as well as Consultations controller


## Developer ##

Alex Farokhyans
