# IdentityApp

[![.NET][ci-badge]][ci-status]

Based on the Web Application built in the Book ['Pro ASP.NET Core Identity'](https://www.apress.com/la/book/9781484268575) by Adam Freeman (Apress, 2021).

This application showcases features that can be accessed by anyone, features that can be accessed only once a user signs in, and features that can be accessed only by administrators.

## Prerequisites

- Visual Studio 2019 v16.10.x
- smtp4dev v3.1.3.2

## Getting started

1. Clone the project.
1. Open the solution file `IdentityApp.sln` with Visual Studio 2019.
1. Right click the `IdentityApp` solution node in the Solution Explorer tool window and click 'Restore Client-Side Libraries' option.
1. (Optional) Open a terminal and enter `smtp4dev` to start smtp4dev local SMTP and leave the terminal open.
1. Press F5 to start the application.
1. Open your browser and go to <https://localhost:44350> (the browser will not launch automatically).

## License

[MIT License](LICENSE)

Copyright (c) 2021 Felipe Romero

[ci-status]: https://github.com/feliperomero3/AspNetCoreIdentityBook/actions/workflows/dotnet.yml
[ci-badge]: https://github.com/feliperomero3/AspNetCoreIdentityBook/actions/workflows/dotnet.yml/badge.svg
