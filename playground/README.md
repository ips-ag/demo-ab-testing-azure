# A/B Testing Playground

## Introduction

The A/B Testing Playground is a comprehensive suite designed to facilitate A/B testing experiments in a controlled environment. It consists of two main components: an API that serves as the backend, providing endpoints for managing and tracking A/B tests, and a Client application that offers a user-friendly interface for setting up and monitoring these tests. This setup aims to simplify the process of conducting A/B tests, making it accessible to users with varying levels of technical expertise.

## Getting Started

To get started with the A/B Testing Playground, follow these steps:

1. **Clone the Repository**: First, clone this repository to your local machine using the following command:

```
git clone <repository-url>
```

Replace `<repository-url>` with the actual URL of this repository.

2. **Start the API**: Navigate to the API directory and run the following commands to start the API server:
   This will start the API server on `http://localhost:5103`. You can access the Swagger UI at `http://localhost:5103/swagger/index.html` to explore the available endpoints.

```
cd API dotnet restore dotnet run
```

3. **Start the Client**: Open a new terminal window, navigate to the client directory, and run the following commands to start the client application:

```
cd CLIENT npm install npm start
```

This will start the client application on `http://localhost:4200`.

## Prerequisites

Before you can run the A/B Testing Playground, ensure you have the following prerequisites installed on your system:

- **.NET 7.0 SDK**: Required to run the API. You can download it from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).
- **Node.js and npm**: Required to run the client application. Download them from [https://nodejs.org](https://nodejs.org).

With these prerequisites in place, you're ready to start using the A/B Testing Playground to conduct your A/B testing experiments.

## Credits

Many thanks [Rahul Sahay](https://www.udemy.com/user/rahulsahay-2/) for bringing [Building FullStack App using .NetCore, Angular & ChatGPT
](https://www.udemy.com/course/building-fullstack-app-using-netcore-angular-chatgpt/#instructor-1) course to life. If you found the course useful, please consider to buy the course to support him.

## Contributing

We welcome contributions from the community! If you'd like to contribute to the A/B Testing Playground, please follow our contribution guidelines.

## License

The A/B Testing Playground is licensed under the MIT License. Feel free to use, modify, and distribute the code as per the terms of the license.

## Feedback

We value your feedback! If you have any suggestions, bug reports, or feature requests, please open an issue on our GitHub repository.