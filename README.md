## TODOs

- TestStringResponse
- TestStringResponseExtensions (ToTestResponse with reading)
- TestDtoResponse
  - indented Json response
- TestFileResponse
- ShouldSucceed(withStatus)
- exception with response details
- extensibility (exception details, custom dtos,)
- rename project to TestHttpResponse maybe
- add docs 
  - XML
  - and Readme with concepts and how to use
- coverage
- nuget package
- try TUnit
- use netstandard for library for compatibility

## Notes
- testResponse needs to be read before using
  - but there is an extension that makes this easy

- testResponse does not fail during reading response by default (different status codes, or different models)
  - but you can do assertions when you need it
      - but there is a method that helps to assert _needed_ status code 
      - and an extension that helps to ensure test does not continue after failed request
      - and exception returns useful info like statusCode and actual response

- testResponse offers classes only for base return scenarios (string, dto, stream, file)
  - but allows to create your own testResponse class for your case