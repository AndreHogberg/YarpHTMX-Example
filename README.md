A small repository where i test using YARP and getting views from a different API and using htmx

There is also authentication enabled but can easily remove by removing the configs and AuthorizationPolicy in appsettings.json


App1 = API that serves the htmx snippets

App2 = Razor Pages that consume the App1 snippets

YarpHtmx = Our reverse proxy

# What is happening:
App2 index page which is http://localhost:5128/ will create a http get to http://localhost:5128/app1/hello when the sites loads.
App1 hello route will respond with a html response.

