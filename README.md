# HotDesk
HotDesk is WebAPI buit on .Net Core 3.1. By this API you can manage booking of desks in the office.

# Models
## Entities
There are three main Entyties:
- User -  To save user data (Email, FirstName, LastName, Role, PasswordHash)
- Location - To save location data (LocationName)
- Desk - To save desk data (Location, Unavailable)

## ServiceResponse
ServiceResponse is a special model to save Service responces of different type. It has Data that will be returned, Message of response and Success flag.

## Dtos
I used different Data tranfer objects (Dtos) in order to meke it easier to buid requests and responses

# Database
In order to complete project in a few days I decided to use InMemoryDatabase. It has some disadvantages. For example, if you rerun your instance all data that you wrote will be deleted (except for mocked data).

# Mocked Data 
There are some mocked data which will help you if you don't want to create your own data. You can find it in `HotDesk/DataAccess/DataMock.cs`.

# Authentication
For authentication purposes I used JWT token authorization. For almost all requests you need to add header attribute `Authorization` with value `bearer token`, you can get your token after logging in to the system.

# Controllers
## Authentication Controller
All methods are with [AllowAnonymous] attribute so you do not need write your token into the header.
- [HttpPost] /api/Authentication
  - in the body of the request you need to enter email of the user and password. For example,
  ```
  {
    "login":"SAkhmetkaliyev",
    "password":"123"
  }
  ```
  returns information about user with token. For example,
  ```
  {
    "id": 2,
    "login": "EMusk",
    "firstName": "Elon",
    "lastName": "Musk",
    "token":          "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjIiLCJnaXZlbl9uYW1lIjoiRWxvbiIsImZhbWlseV9uYW1lIjoiTXVzayIsIm5iZiI6MTY2MDQ4NDI0NiwiZXhwIjoxNjYwNTcwNjQ2LCJpYXQiOjE2NjA0ODQyNDZ9.RDMkUG5HIVXTuuJqBnPc2lUr3zqUCNzPBqxZ-NiL1kM"
  }
  ```
  
- [HttpPost] /api/Authentication/register
  - in the body of the request you need to enter email, first name, last name, role and password. Role can be any string, but if you want to create user to manage desks and locations please enter `Admin` with capital letter
  ```
  {
    "email":"AA",
    "firstname":"A",
    "lastname":"A",
    "role":"Admin",
    "password":"123"
  }
  ```
  
## Location controller
- [HttpGet] /api/Location/
   - lists all locations. For example,
   ```
   {
    "data": [
        {
            "locationId": 1,
            "locationName": "Main Hall"
        },
        {
            "locationId": 2,
            "locationName": "Conference Hall"
        },
        {
            "locationId": 3,
            "locationName": "Coworking Center"
        }
    ],
    "success": true,
    "message": null
  }
  ```
  
- [HttpPost] /api/Location/
  - adds new location. In the body of request enter name of a new Location. For example,
  ```
  {
    "locationName":"Conference Hall4"
  }
  ```
  
 - [HttpDelete] /api/Location/{locationId}
  - deletes location with id `locationId`. 
  - You cannot delete location if you are not an `Admin`
  - You cannot delete location if there is a desk on this location
  
 ## Desk Controller
 - [HttpGet] /api/Desk/
  - lists all desks. For example,
  ```
  {
    "data": [
        {
            "deskId": 1,
            "location": {
                "locationId": 1,
                "locationName": "Main Hall"
            },
            "unavailable": false
        },
        {
            "deskId": 2,
            "location": {
                "locationId": 2,
                "locationName": "Conference Hall"
            },
            "unavailable": true
        }
    ],
    "success": true,
    "message": null
  }
  ```
- [HttpGet] /api/Desk/available
  - lists all available desks. For example,
  ```
  {
    "data": [
        {
            "deskId": 1,
            "location": {
                "locationId": 1,
                "locationName": "Main Hall"
            },
            "unavailable": false
        }
    ],
    "success": true,
    "message": null
  }
  ```
  
- [HttpGet] /api/Desk/unavailable
  - lists all unavailable desks. For example,
  ```
  {
    "data": [
        {
            "deskId": 2,
            "location": {
                "locationId": 2,
                "locationName": "Conference Hall"
            },
            "unavailable": true
        }
    ],
    "success": true,
    "message": null
  }
  ```

- [HttpGet] /api/Desk/{locationId}
  - lists desks which is in location with id `locationId`. For example
  ```
  {
    "data": [
        {
            "deskId": 1,
            "location": {
                "locationId": 1,
                "locationName": "Main Hall"
            },
            "unavailable": false
        }
    ],
    "success": true,
    "message": null
  }
  ```
  
  - returns error if there is no location with id `locationId`

- [HttpPost] /api/Desk/
  - adds new desk. In the body of request enter location id
  ```
  {
    "locationId":4
  }
  ```
  - returns error message if you are not an `Admin` or location does not exist

- [HttpDelete] /api/Desk/{deskId}
  - You cannot delete desk if you are not an `Admin` or desk has future reservation or if desk does not exist

- [HttpPut] /api/Desk
  - Changes unavailable flag of desk. In the body of request enter desk id and unavailable value. For example,
  ```
  {
    "deskId":2,
    "unavailable":false
  }
  ```
  - you cannot make it unavailable if it has future reservations
  - you cannot change unavailable flag if desk does not exist

## Reservation Controller

- [HttpGet] api/reservation
  - lists all reservation of logged user
  ```
  [
    {
        "reservationId": 1,
        "from": "2022-08-12T00:00:00",
        "to": "2022-08-13T00:00:00",
        "cancelled": false,
        "user": {
            "userId": 2,
            "firstName": "Elon",
            "lastName": "Musk",
            "email": "EMusk",
            "role": "Employee",
            "passwordHash": "$2a$11$SwhASwfgpRwpA4nY77L8GuuDJODTnMl3L7/lwWAq1KQqkWfusZHdG"
        },
        "desk": {
            "deskId": 1,
            "location": {
                "locationId": 1,
                "locationName": "Main Hall"
            },
            "unavailable": false
        }
    }
  ]
  ```
- [HttpGet] api/reservation/{locationId}
  - lists all reservations related to the location with location id `locationID'. If you are Employee
    ```
    {
    "data": [
        {
            "deskId": 1,
            "from": "2022-08-12T00:00:00",
            "to": "2022-08-13T00:00:00"
        }
    ],
    "success": true,
    "message": null
    }
    ```
  - if you are admin
  ```
  {
    "data": [
        {
            "deskId": 1,
            "userId":2,
            "from": "2022-08-12T00:00:00",
            "to": "2022-08-13T00:00:00"
        }
    ],
    "success": true,
    "message": null
  }
  ```
- [HttpPost] api/reservations
  - adds new reservation. Body example
  ```
  {
    "deskId":1,
    "from": "2022-08-18",
    "to": "2022-08-19"
  }
  ```
  - you cannot add a reservation if there are overlaping reservations
  - you cannot add a reservation if there is not such desk
  - you cannot add a reservation if there are not future dates
  - you cannot add a reservation if `from` date is later than `to` date

- [HttpPut] api/reservation
  - updates desk of reservation. Body example
  ```
  {
    "reservationId":2,
    "deskId": 2
  }
  ```
  - you cannot update a reservation if there are overlaping reservations
  - you cannot update a reservation if there is not such desk
  - you are not owner of reservation
  
# Unit testing
I tested services in this project. I didn't have much time to finish unit testing `ReservationService`. I used `XUnitTest`
 
