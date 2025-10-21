# Project Structure and Best Practices

This document outlines the recommended folder structure and best practices for the Storier.Cli### Narrative System
The application uses a structured memory system with Markdown files to maintain story continuity. The current world is based on the STALKER universe - a post-apocalyptic Zone filled with anomalies, mutants, and artifacts. World and character information are loaded from dedicated Markdown files in the Memory folder.roject, a .NET console application for AI-powered storytelling.

## Folder Structure

```
storier/
├── .git/
├── .gitignore
├── README.md
├── Storier.sln
├── docs/
│   ├── MemoryExample/
│   │   ├── system-prompt.md
│   │   ├── world/
│   │   │   ├── README.md
│   │   │   └── cordon.md
│   │   ├── characters/
│   │   │   ├── README.md
│   │   │   └── kowalski.md
│   │   └── missions/
│   │       ├── README.md
│   │       ├── dostawa-medykamentow.md
│   │       └── kowalski/
│   │           └── pomoc-nowicjusza.md
│   └── project-structure.md
├── src/
│   └── Storier.Cli/
│       ├── Memory/
│       │   ├── system-prompt.md
│       │   ├── memory/
│       │   │   └── README.md
│       │   ├── missions/
│       │   │   ├── README.md
│       │   │   ├── dostawa-medykamentow.md
│       │   │   └── kowalski/
│       │   │       └── pomoc-nowicjusza.md
│       │   ├── world/
│       │   │   ├── README.md
│       │   │   └── cordon.md
│       │   └── characters/
│       │       ├── README.md
│       │       └── kowalski.md
│       ├── Models/
│       │   └── Settings.cs
│       ├── Services/
│       │   └── AIService.cs
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── Storier.Cli.csproj
```

### Explanation of Folders

- **.git/**: Git repository metadata
- **docs/**: Documentation files
  - **MemoryExample/**: Example structure for narrative memory files
  - **project-structure.md**: Project structure and best practices
- **src/**: Source code
  - **Storier.Cli/**: Main project folder
    - **Memory/**: Narrative memory files (.md) for story continuity
      - **system-prompt.md**: Base system prompt for the AI narrator
      - **memory/**: Reserved for future general memory files
      - **missions/**: Mission and quest information
        - **README.md**: Mission overview
        - ***.md**: General mission files
        - **character/**: Character-specific missions
      - **world/**: World description and locations
        - **README.md**: Main world description
        - ***.md**: Individual location files
      - **characters/**: Character information
        - **README.md**: Character overview
        - ***.md**: Individual character files
    - **Models/**: Data models and configuration classes
      - **Settings.cs**: Configuration classes
    - **Services/**: Business logic services
      - **AIService.cs**: AI interaction service
    - **Program.cs**: Application entry point
    - **appsettings.json**: Base configuration
    - **appsettings.Development.json**: Development overrides
    - **Storier.Cli.csproj**: Project file

## Best Practices

### 1. Namespace Organization
- Use file-scoped namespaces (C# 10+ feature)
- Namespace should match the project name: `Storier.Cli`
- Avoid global namespace pollution

### 2. Configuration Management
- Use `appsettings.json` for base configuration
- Use `appsettings.{Environment}.json` for environment-specific overrides
- Bind configuration to strongly-typed classes
- Add sensitive files to `.gitignore`

### 3. Dependency Injection and Services
- Use Microsoft.Extensions.DependencyInjection for service registration
- Inject IOptions<T> for configuration in services
- Register services with appropriate lifetimes (Singleton for shared state)
- Resolve services through IServiceProvider

### 4. Error Handling
- Use null-coalescing and null-conditional operators
- Throw meaningful exceptions with descriptive messages
- Validate configuration at startup

### 5. Code Organization
- Separate concerns: UI (Program.cs), Business Logic (Services), Data (Settings)
- Use async/await for I/O operations
- Follow C# naming conventions

### 6. Project Configuration
- Use `<ImplicitUsings>enable</ImplicitUsings>` for cleaner code
- Enable nullable reference types: `<Nullable>enable</Nullable>`
- Copy configuration files to output directory

### 7. Version Control
- Commit source code and project files
- Ignore build outputs, user-specific files, and secrets
- Use meaningful commit messages

### 8. Build and Deployment
- Ensure the project builds without warnings
- Test configuration loading
- Use appropriate .NET SDK version

## Future Enhancements

- Add unit tests in a separate test project
- Implement logging with Microsoft.Extensions.Logging
- Add command-line argument parsing
- Consider dependency injection container for complex scenarios
- Add API documentation comments
- Expand narrative system with more memory files and dynamic world building

### Narrative System
The application uses a structured memory system with Markdown files to maintain story continuity. The current world is based on the STALKER universe - a post-apocalyptic Zone filled with anomalies, mutants, and artifacts.

### System Prompt Rules
1. Maintain world consistency and remember past events
2. Use vivid descriptions of surroundings, senses, and emotions
3. Create natural character dialogues
4. Never break the fourth wall or reveal AI nature
5. Build tension and curiosity in the story
6. Reference narrative memory files when available

## Tools and Technologies

- .NET 9.0
- Microsoft.SemanticKernel for AI integration
- Microsoft.Extensions.Configuration for settings
- Microsoft.Extensions.DependencyInjection for DI
- Git for version control

This structure provides a solid foundation for maintainable, scalable .NET console applications.