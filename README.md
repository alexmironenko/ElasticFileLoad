ElasticFileLoad
===============

This is sample console test application to load json file fith User list to ElasticSearch. 

User entity has the following fields:
  - Id (integer)
  - Name (string)
  - Dob (date)
  - Score (float)

I haven't seen your code formatting standards, code pattern requirements, commenting, logging and exception handling strategies, so default formatting with minimal comments, validations and minimal functionality implemented in this project.

Quick Start
===========

Start ElasticSearch and Kibana containers:

    docker-compose up

Update ElasticSearch connection details and index name in:

    appsettings.json

Run file load:

    ElasticFileLoad.exe userlist.json
