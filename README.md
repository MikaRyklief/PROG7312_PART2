# Municipal Services Application

A web-based municipal services platform built with ASP.NET Core MVC that allows citizens to report issues and track their submissions. The application demonstrates the use of data structures including linked lists and queues for data management.

## Features

- **Issue Reporting**: Citizens can report municipal issues with location, category, description, and optional file attachments
- **Data Persistence**: Issues are stored using a custom linked list implementation and persisted to JSON files
- **Engagement Tracking**: Progress bar and recent messages system using queue data structure
- **Responsive Design**: Modern UI with Bootstrap styling inspired by Western Cape Government
- **File Upload**: Support for image and document attachments

## Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript
- **Data Storage**: Custom linked list implementation with JSON persistence
- **File Storage**: Local file system for uploads

## Data Structures Used

### Linked List Implementation
- **Purpose**: Custom linked list for storing issue reports
- **Location**: `LinkedListIssueRepository.cs`
- **Benefits**: Demonstrates dynamic memory allocation and sequential data access

### Queue Implementation
- **Purpose**: FIFO queue for recent engagement messages
- **Location**: `LinkedListIssueRepository.cs`
- **Benefits**: Maintains chronological order of user feedback messages

## Project Structure

```
PROG7312_Part1_POE_ST10318273/
├── Controllers/
│   ├── HomeController.cs          # Home and Privacy page controllers
│   └── IssuesController.cs        # Issue reporting and management
├── Data/
│   ├── IIssueRepository.cs        # Repository interface
│   └── LinkedListIssueRepository.cs # Custom linked list implementation
├── Models/
│   └── issue.cs                   # Issue data model
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml          # Landing page with service cards
│   │   └── Privacy.cshtml        # Privacy policy page
│   ├── Issues/
│   │   └── Report.cshtml         # Issue reporting form
│   └── Shared/
│       ├── _Layout.cshtml        # Main layout template
│       └── _Layout.cshtml.css    # Layout-specific styles
├── wwwroot/
│   ├── css/
│   │   └── site.css              # Application styles
│   ├── img/
│   │   └── logo.svg              # Application logo
│   └── uploads/                  # File upload directory
├── Data/
│   └── issues.json               # Persistent issue storage
└── Program.cs                    # Application startup and configuration
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK 
- Visual Studio 2022 
- Web browser

### Installation

1. **Clone the repository**
   ```bash
   git clone <https://github.com/MikaRyklief/PROG7312_Part1_POE_ST10318273.git>
   cd PROG7312_Part1_POE_ST10318273
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open in browser**
   Navigate to `https://localhost:5001` or `http://localhost:5000`

### Configuration

The application uses the following configuration:
- **Data Storage**: Issues are persisted to `Data/issues.json`
- **File Uploads**: Stored in `wwwroot/uploads/`

## Usage

### Reporting an Issue

1. Navigate to the "Report Issue" page
2. Fill in the required fields:
   - Reporter Name (optional)
   - Location (required)
   - Category (required)
   - Description (required)
   - File attachment (optional)
3. Click "Submit" to save the issue
4. View the updated engagement progress bar

### Data Management

- **Adding Issues**: Issues are automatically added to the linked list and persisted
- **Retrieving Issues**: Use the repository methods to access stored data
- **File Management**: Uploaded files are stored with unique names to prevent conflicts

## Code Documentation

### Key Classes

#### `Issue` Model
Represents a municipal issue report with properties for identification, location, description, and metadata.

#### `LinkedListIssueRepository`
Custom repository implementation using a linked list for data storage. Includes:
- Node-based linked list structure
- Queue for engagement messages
- JSON serialization for persistence

#### `IssuesController`
Handles HTTP requests for issue reporting, including:
- Form validation
- File upload processing
- Data persistence
- Engagement tracking

## Data Flow

1. **User Input**: Citizen fills out the issue reporting form
2. **Validation**: Server-side validation ensures required fields are present
3. **File Processing**: Optional file uploads are saved to the uploads directory
4. **Data Storage**: Issue is added to the linked list and persisted to JSON
5. **Engagement Update**: Progress bar and recent messages are updated
6. **User Feedback**: Success message is displayed to the user

## File Structure Details

### Controllers
- **HomeController**: Manages home page and privacy policy
- **IssuesController**: Handles issue reporting, validation, and file uploads

### Data Layer
- **IIssueRepository**: Interface defining data access methods
- **LinkedListIssueRepository**: Concrete implementation using linked list and queue

### Models
- **Issue**: Data model representing a municipal issue report

### Views
- **Index.cshtml**: Landing page with service cards and hero section
- **Report.cshtml**: Issue reporting form with validation and engagement tracking
- **Privacy.cshtml**: Privacy policy page
- **_Layout.cshtml**: Main layout with navigation and footer

## Styling

The application uses a deep blue color scheme inspired by the Western Cape Government website:
- Primary color: `#003366` (deep blue)
- Secondary color: `#00509e` (lighter blue)
- Text color: `#ffffff` (white) for contrast
- Accent color: `#e6f0ff` (light blue) for hover states

## Future Enhancements

- User authentication and authorization
- Issue status tracking and updates
- Email notifications for issue updates
- Admin dashboard for issue management
- Database integration (SQL Server/PostgreSQL)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add appropriate comments and documentation
5. Test your changes
6. Submit a pull request

## License

This project is part of PROG7312 coursework and is for educational purposes.

## Contact

For questions or support, please contact me st10318273@vcconnect.edu.za.

---