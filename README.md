# QR-code-reader

In order to keep it simple, the validator and handler are located in the same class with the command DecodeQrImage.
I used the MediatR library for sending commands(CQRS) but my architecture is not quite CQRS-compliant as my command returns the encoded data but. I should have used a query! 
I added Swagger for testing the API!
