# DaffaCatering-API
Backend API for a catering business management system (ASP.NET Core + EF Core), demonstrating modernization from a legacy WinForms/.NET Framework architecture.

A business operations management system for a catering business, rebuilt from an older desktop architecture (WinForms/.NET Framework) into a modern backend based on **ASP.NET Core Web API**.

## About the Project
This project covers the catering business process: master data management (raw materials, recipes, menus), purchase and sales transactions, inventory management using the FEFO (First Expired, First Out) method, and operational reports.

Originally developed as a desktop application (WinForms) to understand business processes from scratch, this project is an advanced iteration that modernizes its architecture into a RESTful API, separating the backend logic from the user interface so it can be accessed by various types of clients (web, mobile, or desktop) via HTTP endpoints.

## Key Features
- Master data management (raw materials, recipes, menus)
- Purchases from suppliers
- Sales to customers
- Inventory management using FEFO logic
- Role-based access control
- Operational reports

## Tech Stack
- **Language:** C#
- **Framework:** ASP.NET Core Web API (.NET 10)
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Testing:** xUnit
- **API Documentation:** Swagger / OpenAPI

## Arsitektur
This backend is designed using a client-server architecture: all business logic and data access are handled on the API side, while the user interface (frontend) is developed separately and retrieves data via HTTP requests.

## Status
Still under active development.

## Author
Developed by Daffa Shiddiq Hidayat as a personal portfolio project, building on the concept from his Information Systems thesis project.
