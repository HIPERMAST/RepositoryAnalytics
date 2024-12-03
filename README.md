# RepositoryAnalytics 📊

### Unity Project for GitHub Repository Data Visualization

> A Unity project that visualizes GitHub repository data, including branches, pull requests, issues, and contributors. Uses the GitHub API to fetch real-time data and display it in an interactive 3D environment.

![Unity Version](https://img.shields.io/badge/Unity-6-brightgree)
![Python](https://img.shields.io/badge/Python-3.12.0-yellow)
![Git](https://img.shields.io/badge/Git-2.44.0-orange)


---

## Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Installation](#installation)
4. [Dependencies](#dependencies)
5. [Unity Project Structure](#unity-project-structure)
6. [Scripts](#scripts)
7. [Prefabs and Materials](#prefabs-and-materials)
8. [API Integration](#api-integration)
9. [Icons and UI](#icons-and-ui)
11. [Additional Documentation](#additional-documentation)

---

## Overview

RepositoryAnalytics is a Unity-based project that visualizes data from GitHub repositories using GitHub's API. It allows users to interact with and explore branches, issues, pull requests, and member contributions in a 3D environment. 

This project is ideal for:
- Teams looking to understand repository activity visually
- GitHub users interested in 3D visualizations of their project metrics
- Developers wanting to experiment with Unity's UI and API integration capabilities

## Features 🌟

- **Dynamic Page Transitions**: Smooth animations between pages.
- **Pagination System**: Navigate through GitHub data with a custom pagination interface.
- **GitHub API Integration**: Fetch real-time data from repositories.
- **Unity UI Customization**: UI elements, icons, and labels for easy interaction.
- **3D Data Representation**: Interactive 3D objects for branches, issues, pull requests, and members.

## Installation 🚀

Follow these steps to set up the project:

### **1. Install Unity**

- **Download Unity Hub**:
    - Go to the [Unity Download Page](https://unity.com/download) and download Unity Hub.

- **Install Unity Editor**:
    - Open Unity Hub and go to the `Installs` tab.
    - Click `Add` and select **Unity Version 6**. Follow the installation prompts.

- **Add Modules**:
    - During installation, ensure to include modules such as:
      - **Windows Build Support** (if using Windows).
      - **Mac Build Support** (if using macOS).

- **Verify Installation**:
    - After installation, confirm that Unity is properly installed by opening Unity Hub and creating a new project as a test.

### **2. Install Required Dependencies**

To run the project successfully, both Python and Gradle are required. Follow these steps to install and set them up:

#### **Install Python**
1. **Download Python**:
   - Visit the [Python official website](https://www.python.org/downloads/).
   - Download the latest stable version of Python (ensure it's at least Python 3.7).

2. **Install Python**:
   - Run the downloaded installer.
   - During installation:
     - **Check the box** for "Add Python to PATH".
     - Select "Customize installation" and ensure `pip` is selected.

3. **Verify Installation**:
   - Open a terminal or command prompt and type:
     ```bash
     python --version
     ```
     You should see the installed Python version.

4. **Install Required Python Libraries**:
   - Navigate to the directory where the script is located.
   - Use `pip` to install the required libraries for the GitHub API script:
     ```bash
     pip install requests
     pip install json
     ```

5. **Test Python Setup**:
   - Run a simple script to ensure Python is working:
     ```bash
     python -c "print('Python is installed successfully!')"
     ```

---

#### **Install Gradle**
1. **Download Gradle**:
   - Visit the [Gradle official website](https://gradle.org/releases/).
   - Download the latest **binary-only** distribution (or the compatible version specified by the project).

2. **Install Gradle**:
   - Extract the downloaded `.zip` file to a directory of your choice (e.g., `C:\Gradle` on Windows or `/opt/gradle` on macOS/Linux).
   - Add the Gradle `bin` directory to your system's `PATH`:
     - **Windows**:
       - Go to `Control Panel > System > Advanced System Settings > Environment Variables`.
       - Add a new system variable `GRADLE_HOME` pointing to the Gradle installation directory (e.g., `C:\Gradle`).
       - Edit the `Path` variable and add `%GRADLE_HOME%\bin`.
     - **macOS/Linux**:
       - Open your terminal and edit the `~/.bashrc`, `~/.zshrc`, or `~/.bash_profile` file:
         ```bash
         export GRADLE_HOME=/opt/gradle
         export PATH=$GRADLE_HOME/bin:$PATH
         ```
       - Save and run `source ~/.bashrc` or `source ~/.zshrc`.

3. **Verify Installation**:
   - Open a terminal or command prompt and type:
     ```bash
     gradle -v
     ```
     You should see the installed version of Gradle and system information.

4. **Configure Gradle in the Project**:
   - Ensure that the `build.gradle` file is present in the project directory.
   - Open a terminal in the project directory and run:
     ```bash
     gradle build
     ```
     This will download any required dependencies and build the project.

 ### **3. Setup GitHub API Token (optional)**
- Go to your GitHub account and navigate to Settings > Developer Settings > Personal Access Tokens.
- Generate a new token with the required permissions (e.g., repo, read:org).
- This token will give you more insights about the project.


### **4. Clone and Run the Repository**

Follow these steps to clone the project and run it in Unity:

1. **Clone the Repository**:
   - Open a terminal or command prompt.
   - Navigate to the directory where you want to store the project.
   - Run the following command:
     ```bash
     git clone https://github.com/HIPERMAST/RepositoryAnalytics.git
     cd RepositoryAnalytics
     ```

2. **Open the Project in Unity**:
   - Launch Unity Hub.
   - Click `Open` and navigate to the folder where you cloned the repository.
   - Select the folder to load the project into Unity.

3. **Set Up GitHub API Token**:
   - Go to your GitHub account:
     - Navigate to **Settings > Developer Settings > Personal Access Tokens**.
     - Generate a new token with permissions such as `repo` and `read:org`.
   - Add the token to the script where the GitHub API is configured. Typically, this will involve replacing a placeholder or updating a configuration file.

4. **Run the Project**:
   - In Unity, open the main scene file or the specified entry point for the project.
   - Press the `Play` button in the Unity Editor to start the project.
   - Interact with the 3D visualization to explore the repository's data.
  

---

## Dependencies 🧩

The following dependencies are required for this project:

- **Unity Version**: 6
- **Newtonsoft.Json**: For handling JSON data from the GitHub API.
- **GitHub API Token**: Required to authenticate requests to the GitHub API.

---

## Unity Project Structure 🏗️

The project structure is organized as follows:

```plaintext
Assets/
├── Scripts/
│   ├── API/
│   │   └── GitHubAPIHandler.cs           # Handles all GitHub API calls
│   ├── Spawners/
│   │   ├── BranchSpawn.cs                # Branch data handler
│   │   ├── IssueSpawn.cs                 # Issue data handler
│   │   ├── MemberSpawn.cs                # Member data handler
│   │   └── PullRequestSpawn.cs           # Pull Request data handler
│   └── UI/
│       ├── Pagination.cs                 # Pagination interface implementation
│       └── UIManager.cs                  # Manages UI elements and transitions
├── Prefabs/
│   ├── CubePrefab.prefab                 # Prefab for branches
│   ├── IssuePrefab.prefab                # Prefab for issues
│   └── PullRequestPrefab.prefab          # Prefab for pull requests
└── Icons/
    ├── PullRequestIcon.png               # Icon for Pull Requests
    ├── BranchIcon.png                    # Icon for Branches
    └── IssueIcon.png                     # Icon for Issues
```

---

## Key Sections 🏗️

Each major folder in `Assets` is structured to keep related components together:

- **Scripts**: Contains code for API interaction, data visualization, and Unity UI management.
- **Prefabs**: Stores prefab assets for branches, issues, pull requests, etc.
- **Materials**: Contains materials used for visual styling.
- **Icons**: Holds icons for GitHub data types like issues and pull requests.

---

## Scripts 📜

# GitHubAPIHandler.cs

- **Purpose**: Fetches data from GitHub's API and parses it into usable JSON data.
- **Methods**:
  -  `GetRepositoryData()`: Fetches general data about the repository.
  -  `GetIssues()`: Retrieves all issues in the repository.
  -  `GetPullRequests()`: Retrieves all pull requests in the repository.
  -  `GetBranches()`: Retrieves all branches in the repository.
 
# Spawner Scripts

Each script handles spawning a specific type of data (branch, issue, member, or pull request) in the Unity scene.

- `BranchSpawn.cs`, `IssueSpawn.cs`, `MemberSpawn.cs`, `PullRequestSpawn.cs`
  - `Purpose`: Each script is responsible for displaying a specific type of GitHub data in the Unity environment.
  - `Animation`: Includes sliding animations for smooth transitions between pages.
  - `Pagination`: Implements pagination for efficient data handling.
 
---

## Prefabs and Materials 🎨

# GitHubAPIHandler.cs

- **CubePrefab**: Represents branches as 3D cubes in the scene. Customizable with different materials for a unique visual style.
- **IssuePrefab**: PullRequestPrefab: Prefabs used to display issues and pull requests in 3D. Each prefab includes components for displaying key information about the data.
- **Materials**: The project includes a `WallMaterial` used for background or boundary walls in the scene. Customizable to match the project's aesthetic.
 
---

## API Integration 🌐

# GitHub API Setup

1. **Authentication**: Create a personal access token from your GitHub account.
2. **Endpoints Used**:
  - `GET /repos/{owner}/{repo}/issues`:Retrieves repository issues.
  - `GET /repos/{owner}/{repo}/pulls`: Retrieves pull requests.
  - `GET /repos/{owner}/{repo}/branches`: Retrieves branches.

# Data Flow in Unity

- `GitHubAPIHandler.cs`: Makes requests to GitHub API and parses JSON responses.
- **Spawner Scripts**: Each data type (branch, issue, etc.) has a corresponding spawner script that manages its visualization in the Unity scene.
- **UI Manager**: Controls pagination and transitions between pages, making it easy to navigate through large datasets.
 
---

## Icons and UI 🎨

- **Icons**: Icons for each data type (Issue, Branch, Pull Request) are located in the `Assets/Icons` directory. Each icon helps differentiate data types visually in the UI.
- **UI Layout**:
  - **Text Fields**: Labels and fields to display details about each GitHub entity (title, assignees, status, etc.).
  - **Pagination Controls**: Buttons for navigating between pages of data.
  
---

## Additional Documentation 📚

Consider reading additional documentation for more information on this project:
- [Unity Documentation]([#heading-ids](https://docs.unity3d.com/6000.0/Documentation/Manual/UnityManual.html)): Learn more about Unity's features and API.
- [GitHub API Documentation](https://docs.github.com/en/rest)): Detailed information about GitHub API endpoints.
- [Newtonsoft.Json](https://www.newtonsoft.com/jsonschema/help/html/Introduction.htm): JSON library for .NET used in this project.
