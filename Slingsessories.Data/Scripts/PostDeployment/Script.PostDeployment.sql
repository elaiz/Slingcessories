/*
--------------------------------------------------------------------------------------
 Post-Deployment Script
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed after the build script.
 Use SQLCMD syntax to include a file in the post-deployment script.
 Example:      :r .\SeedData.sql
 Use SQLCMD syntax to reference a variable in the post-deployment script.
 Example:      :setvar TableName MyTable
               SELECT * FROM [$(TableName)]
--------------------------------------------------------------------------------------
*/

-- Optional: Add seed data or reference data here
-- This script runs after deployment

PRINT 'Post-deployment script executed successfully.';
GO
