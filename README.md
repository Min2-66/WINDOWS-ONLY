# ScholasticaReader - Windows Educational Reading Platform

## Overview
ScholasticaReader is a WPF-based Windows application designed for educational reading, annotation, and interactive learning.

## Features
- 📚 Multi-format book support (PDF, EPUB, Scholastica)
- ✏️ Annotation system (notes, highlights)
- 🎤 Text-to-speech (TTS) support
- 🎯 Flashcard learning tool
- 🧠 Mind mapping functionality
- 📖 Parallel reading mode
- 👨‍🏫 Teacher dashboard (Premium)
- 🔐 License activation system
- 🛡️ Hardware ID-based security

## Tech Stack
- **Framework**: .NET 8.0 with WPF
- **UI**: XAML-based design
- **Database**: SQLite for licensing
- **Libraries**: PdfPig, VersOne.Epub, WebView2

## Getting Started
1. Clone the repository
2. Open in Visual Studio 2022+
3. Restore NuGet packages
4. Build and run

## File Structure
- `Models/` - Data models (Book, Annotation, UserSettings)
- `Services/` - Business logic (BookService, LicenseService, SecurityService)
- `Views/` - XAML windows for features
- `Assets/` - Application resources

## License
This project uses an activation-based licensing system.
