# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.1.0] - 2025-09-25

### Added

- .NET 9.0 support
- New overloads for `GetVariationsAsync()` and `GetBrowseNodesAsync()` with specific resources
- New overloads for `GetItemsAsync()` and `SearchItemsAsync()` with specific resources

### Fixed

- Fixed several potential null reference exceptions
- Fixed line endings in LICENSE.md

### Changed

- Updated RestSharp to version 111.4.0
- Updated test project to target .NET 9.0
- Updated test dependencies to latest versions
  - Microsoft.NET.Test.Sdk 17.14.1
  - MSTest.TestAdapter 3.10.4
  - MSTest.TestFramework 3.10.4

## [3.0.1] - 2024-03-21

### Changed

- Initialized Resources properties in requests with `= Array.Empty<string>()`
- Code cleanup and README.md update
- Removed AwsSignatureVersion4 dependency

## [3.0.0] - 2024-03-21

### Added

- .NET 8.0 support
- Complete implementation of Amazon's PAAPI 5.0
- New `AwsSigner` class for authorization header generation
- Support for customizable resources in requests

### Removed

- Website project removed

## [2.0.0] - 2021-06-26

### Added

- Support for `Offers.Listings.IsBuyBoxWinner` resource
- Improved unit tests

## [1.0.0] - 2021-05-05

### Added

- Initial support for Amazon Product Advertising API (PAAPI 5.0)
- Basic features: product search and advanced search
- Support for multiple Amazon marketplaces (US, UK, DE, etc.)

---

_Note: This CHANGELOG was generated retroactively from Git history. Details of earlier versions may be incomplete._
