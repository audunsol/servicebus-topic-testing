# servicebus-topic-testing
Testing topic and subscriptions on Azure Service Bus with CorrelationFilter on custom properties

Correlation filters are supposed to be faster than SqlFilters, so I would prefer to use them for simple message routing based on a simple property.
I think the documentation on these are somewhat limited, so I just made some test code for myself to make sure correlation filters acted as I expected.

See also https://github.com/Azure/azure-service-bus/issues/92

## What the code does
It is a simple .NET Core Console application that sets up a sender and two subscribers to a topic. The sender puts 10 messages to the topic, alternating the filter properties for subscription A and B (every 2nd message to A and B, and every 4th message to both).
The subscribers sets up a filter based on the given property, and prints out the message content when it is received.

## How to run the sample
To run the sample, you must first (from the Azure Portal, or whatever tool that suits you):
1. create a Service Bus namespace in Azure
1. add a topic on the namespace
1. add two subscriptions on the topic
1. add a shared access policy on the topic, with Send and Listen checked
1. copy one of the connectionstrings from this policy

When subscriptions are added through the portal, they have the default TrueFilter set. The subscriber class initialization code will remove this, and put on a correlation filter based on the filter property from the config instead. Note that if you would like to change the filter, you must update the filter name, else it will be left untouched (I don't want to mess around with the subscription filter rules for every execution, but clearly, this could have been done better...)

Then, fill inn the corresponding connectionstrings and subscription names in appSettings.json (or preferrably in an environment specific file, e.g. appSettings.Development.json, that will be ignored by .gitignore setting).