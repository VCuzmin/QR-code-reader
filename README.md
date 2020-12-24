# QR-code-reader

In order to keep it simple, the validator and handler are located in the same class with command DecodeQrImage
I used MediatR library for sending commands(CQRS) but my architecture is not quite CQRS-compliant as my command returns the encoded data. I should have use a query for that!
