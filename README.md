# Demoblaze_Selenium Automated Tests

This README file provides instructions on how to run the automated tests for the "Demoblaze_Selenium" project using Selenium for UI automation. 
Demoblaze is a fictional e-commerce platform designed for testing and educational purposes. 
It simulates typical online shopping experiences, allowing users to browse products, add items to their cart, and complete purchases. 
This makes it an invaluable tool for hands-on software testing training.

## Project Structure

The project structure includes the following folders:

- **Data**: Contains a CSV file with data needed for test case execution.
- **Pages**: Contains page object classes.
- **Reports**: Contains reports generated after test execution.
- **Tests**: Contains classes with tests.
- **Utils**: Contains classes needed for reading CSV data files, global values, validation messages, and web driver setup class.

The project uses NUnit as the test framework and AventStack ExtentReports for test reporting.

## Setup Instructions

Ensure that the following prerequisites are met:

- Visual Studio is installed on your machine.
- The required NuGet packages are installed as defined in the project files.

## Executing Tests

To run the UI tests, follow these steps:

1. Open Visual Studio and navigate to the project root directory.
2. Build the solution to restore all the NuGet packages.
3. Execute the tests by running the `EndToEndTests` class or using the Test Explorer in Visual Studio.
