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
- [ ] Stream 
  - [ ] File
  - 
### Features (as much response type agnostic as possible)
- [x] Status assertion
- [x] Exceptions*
- [x] HttpResponseMessage converter-reader for client
- [ ] Read + Assert extension that returns deserialized value*
  - [ ] how to make universal
- [ ] Expected status code for client
  (so that client can set up a success status code and test just assert success without having to know the correct status code)
  - [ ] storage for status
  - [ ] converter-reader extension with status code
  - [ ] status assertion for success

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