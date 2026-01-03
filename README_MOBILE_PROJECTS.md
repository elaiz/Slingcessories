# Slingcessories Mobile Projects

This repository contains two mobile applications built with different technologies that connect to the same backend API:

1. **Slingcessories.Mobile.Maui** - .NET MAUI application
2. **slingcessories_mobile_flutter** - Flutter application

## Running Both Projects Simultaneously

**Yes, both projects can run at the same time!** They are separate applications that both connect to the same backend API. You can:

- Run them on the same device/emulator (as separate apps)
- Run them on different devices/emulators
- Compare their functionality side-by-side

Both apps will share the same data from your backend API.

## Prerequisites

### For .NET MAUI:
- .NET 8.0 SDK or later
- Visual Studio 2022 or Visual Studio Code with C# extension
- Android SDK (for Android development)
- Xcode (for iOS development, macOS only)

### For Flutter:
- Flutter SDK (latest stable version)
- Dart SDK (comes with Flutter)
- Android Studio or VS Code with Flutter extension
- Android SDK (for Android development)
- Xcode (for iOS development, macOS only)

## Backend API

Make sure your backend API is running at:
- **Android Emulator**: `http://10.0.2.2:5001`
- **iOS Simulator**: `http://localhost:5001`
- **Physical Device**: Use your computer's IP address (e.g., `http://192.168.1.100:5001`)

Update the `BaseUrl` in:
- **MAUI**: `Slingcessories.Mobile.Maui/Services/ApiService.cs`
- **Flutter**: `slingcessories_mobile_flutter/lib/services/api_service.dart`

## Running the .NET MAUI Project

1. Navigate to the project directory:
   ```bash
   cd Slingcessories.Mobile.Maui
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Run on Android:
   ```bash
   dotnet build -t:Run -f net8.0-android
   ```

4. Or run from Visual Studio by selecting your target platform and pressing F5.

## Running the Flutter Project

1. Navigate to the project directory:
   ```bash
   cd slingcessories_mobile_flutter
   ```

2. Get dependencies:
   ```bash
   flutter pub get
   ```

3. Run the app:
   ```bash
   flutter run
   ```

## Features

Both applications include:

- **Accessories**: View, filter by wishlist, and manage accessories
- **Categories**: View and manage categories
- **Subcategories**: View and manage subcategories
- **Slingshots**: View and manage slingshots
- **Users**: View and manage users

## Project Structure

### .NET MAUI:
```
Slingcessories.Mobile.Maui/
├── Models/          # Data transfer objects
├── Services/        # API service layer
├── ViewModels/      # MVVM view models
├── Pages/           # XAML pages
└── Converters/      # Value converters
```

### Flutter:
```
slingcessories_mobile_flutter/
├── lib/
│   ├── models/      # Data models
│   ├── services/    # API service layer
│   ├── providers/   # State management
│   └── pages/       # UI pages
```

## Notes

- Both projects use the same API endpoints and data models
- The UI/UX is similar but uses each framework's native components
- Both support the same CRUD operations
- Error handling and loading states are implemented in both

