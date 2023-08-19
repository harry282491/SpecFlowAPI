Feature: CategoryE2E

This feature does Category E2E Tests


 Scenario:1 Check data after POST call
	Given the user sends the post request with url
	Then user gets a success response
	And the response contains the correct name

 Scenario:2 Get data by category ID and validate name
    Given the user sends a GET request with the category ID
    Then the Get response contains the correct name and category ID

 Scenario:3 Update category by ID and validate name
    Given the user sends a PUT request with the category ID
    Then the user does a GET response after update by ID and validates the name

 Scenario:4 Delete category by ID and validate the deletion
    Given the user sends a Delete request with the category ID and do an GET to confirm

Scenario:5 NOT OK Case - Delete the non exisitng categeory
    Given the user sends a Delete request with the non exisiting category ID  and validate

Scenario:6 NOT OK Case - Create a category using exisiting Name
    Given the user sends a Pose request with the an exisiting category Name and validate

Scenario:7 Get all category data and validate the datatypes
    Given the user sends a get all request and validate the datatypes