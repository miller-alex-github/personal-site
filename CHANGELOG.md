# Change Log

## [1.10] 2025-04-11
 - Update ZF to v1.2.68	
	- Added: wM-Bus, Parser now supports Kamstrup Multical/QFlow 79h CI-field, RSP from device, compact frame, no data header.

## [1.9] 2025-03-03
 - Update ZF to v1.2.66
	- Migrate to NET9
	- Added: wM-Bus, CompactProfile->IncrementMode->Increments are now supported
	
## [1.8] 2025-03-03
 - Update ZF to v1.2.66
	- Added: [PAR-101] wM-Bus parser, Parser now supports Kamstrup Multical/QFlow (78h CI-field, without header, start immediately with VIF/DIF).
	
## [1.7] 2024-09-18
 - Update ZF to v1.2.56
	- Fix: [ZENAPP-209] M-Bus parser, The management of orthogonal byte in special cases FBh FDh and FDhFDh is implemented incorrectly. 
	
## [1.6] 2024-03-22
 - Update ZF to v1.2.23

## [1.5] 2024-03-13
 - Update ZF to v1.2.20
	
## [1.4] 2024-03-12
 - Migrate to NET8

## [1.3] 2022-02-11
 - Migrate to NET6

## [1.2] 2019-12-01
 - Redesign main layout of footer.
 - Add micro service for appointments (https://appointments.miller-alex.de/docs/index.html).
 - Added unit test scaffold

## [1.1] 2019-11-05
 - Redesign the main layout of the page (logo, menu).
 - Add version information to the main page.
 - Add SQL script to delete all tables from database.

## [1.0] 2019-11-04
 - Initial Release