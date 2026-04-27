using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Collections;

// ⚠ Replace "MVVMLoginApp" below with YOUR project name if different
namespace MVVMLoginApp.Converters
{
    // Returns Visible if the string has text, Collapsed if it is empty.
    // Used to show/hide the error message label automatically.
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

//-- ============================================
//--Mawlers Cinema Complete Setup Script
//-- Run this on any new computer to set up the database
//-- ============================================

//-- Step 1: Create Database
//CREATE DATABASE [Mawlers Cinema] ;
//GO

//-- Step 2: Use the database
//USE [Mawlers Cinema] ;
//GO

//-- Step 3: Create all tables in the correct order
//CREATE TABLE Users (
//    Id       INT IDENTITY(1,1) PRIMARY KEY,
//    Username NVARCHAR(50)  NOT NULL,
//    Password NVARCHAR(50)  NOT NULL
//);

//CREATE TABLE Movies (
//    MovieId  INT IDENTITY(1,1) PRIMARY KEY,
//    Title    NVARCHAR(100) NOT NULL,
//    Genre    NVARCHAR(50)  NOT NULL,
//    Duration NVARCHAR(20)  NOT NULL,
//    Rating   NVARCHAR(10)  NOT NULL
//);

//CREATE TABLE Rooms (
//    RoomId   INT IDENTITY(1,1) PRIMARY KEY,
//    RoomName NVARCHAR(20)  NOT NULL,
//    Capacity INT           NOT NULL
//);

//CREATE TABLE Showings (
//    ShowingId   INT IDENTITY(1,1) PRIMARY KEY,
//    MovieId     INT NOT NULL,
//    RoomId      INT NOT NULL,
//    ShowingDate DATE NOT NULL,
//    ShowingTime TIME NOT NULL,
//    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId),
//    FOREIGN KEY (RoomId)  REFERENCES Rooms(RoomId)
//);

//CREATE TABLE Reservations (
//    ReservationId INT IDENTITY(1,1) PRIMARY KEY,
//    ClientName    NVARCHAR(100) NOT NULL,
//    ShowingId     INT NOT NULL,
//    SeatNumber    INT NOT NULL DEFAULT 0,
//    FOREIGN KEY (ShowingId) REFERENCES Showings(ShowingId)
//);

//--Step 4: Insert starting data
//INSERT INTO Users (Username, Password) VALUES
//('admin', '1234');

//INSERT INTO Movies (Title, Genre, Duration, Rating) VALUES
//('Inception', 'Sci-Fi', '148 min', 'PG-13'),
//('The Dark Knight', 'Action', '152 min', 'PG-13'),
//('Interstellar', 'Sci-Fi', '169 min', 'PG');

//INSERT INTO Rooms (RoomName, Capacity) VALUES
//('Room 1', 50),
//('Room 2', 75),
//('Room 3', 100);

//INSERT INTO Showings (MovieId, RoomId, ShowingDate, ShowingTime) VALUES
//(1, 1, '2026-04-29', '10:00:00'),
//(2, 2, '2026-04-29', '13:00:00'),
//(3, 3, '2026-04-29', '16:00:00');
