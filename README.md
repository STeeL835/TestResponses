TestResponse
============

A family of HttpResponseMessage decorators that make testing easier and more informative

## TODOs

- [x] Base response (with inheritance and extensibility in mind)

### Response types
- [x] String
  - [x] Json<TSuccess>
    - [x] serializer settings
    - [x] As<T> for custom deserialization
- [ ] Empty (for APIs that should return nothing)
- [ ] Stream 
  - [ ] File
- [ ] Multipart form?
- [ ] Grpc?

### Features (as much response type agnostic as possible)

Assertions (thinking of a new feature structure)
  - [x] Assert status code
    - [x] Assert expected status code (that can be set up in client)
  - [ ] Assert response 
    - [x] Deserialized into needed model
    - [ ] Is supported type (empty, json, stream, etc)
      - [ ] Detect real response type and use correct response info
  - [x] Method to assert all

- Response info (status code and well formatted response)
  - [x] ToString 
  - [x] Exceptions

- Usability
  - [x] ReadAs to read from HttpMessageResponse Task 
  - [x] extension that decorates TestResponse Task, and asserts all, and returns deserialized value (for every type)
    - [ ] now make it source generated for every type

### Repository
- [ ] Docs for public classes
- [ ] Readme
- [ ] Nuget
- [ ] CI
- [ ] try to switch to TUnit
- [ ] netstandard for library
- [x] Example project
  - [ ] Fill it
- [ ] Rename project to TestResponse

### Other

- [ ]




## Notes
- testResponse needs to be read before using
  - but there is an extension that makes this easy

- testResponse does not fail during reading response by default, allowing you to check things without catching exceptions
  - but you can do assertions when you need it:
      - there is a method that helps to assert _needed_ status code 
      - AsDto throws a deserialization exception only on access
      - there is an extension that asserts success to ensure test does not continue after failed request
      - and exception returns useful info like statusCode and actual response

- testResponse offers classes only for base return scenarios (string, dto, stream, file)
  - but allows to create your own testResponse class for your case

- Doesn't use CancellationTokens because typically tests do not use them