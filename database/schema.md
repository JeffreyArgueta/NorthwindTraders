## Table Employees
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| EmployeeID | int | **PK**, Identity (1,1), Not Null |
| LastName | nvarchar(20) | Not Null |
| FirstName | nvarchar(10) | Not Null |
| Title | nvarchar(30) | Null |
| TitleOfCourtesy | nvarchar(25) | Null |
| BirthDate | datetime | Null, **CK_Birthdate** (BirthDate < GETDATE()) |
| HireDate | datetime | Null |
| Address | nvarchar(60) | Null |
| City | nvarchar(15) | Null |
| Region | nvarchar(15) | Null |
| PostalCode | nvarchar(10) | Null |
| Country | nvarchar(15) | Null |
| HomePhone | nvarchar(24) | Null |
| Extension | nvarchar(4) | Null |
| Photo | image | Null |
| Notes | ntext | Null |
| ReportsTo | int | FK → Employees(EmployeeID) |
| PhotoPath | nvarchar(255) | Null |

## Table Categories
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| CategoryID | int | **PK**, Identity (1,1), Not Null |
| CategoryName | nvarchar(15) | Not Null |
| Description | ntext | Null |
| Picture | image | Null |

## Table Customers
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| CustomerID | nchar(5) | **PK**, Not Null |
| CompanyName | nvarchar(40) | Not Null |
| ContactName | nvarchar(30) | Null |
| ContactTitle | nvarchar(30) | Null |
| Address | nvarchar(60) | Null |
| City | nvarchar(15) | Null |
| Region | nvarchar(15) | Null |
| PostalCode | nvarchar(10) | Null |
| Country | nvarchar(15) | Null |
| Phone | nvarchar(24) | Null |
| Fax | nvarchar(24) | Null |

## Table Shippers
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| ShipperID | int | **PK**, Identity (1,1), Not Null |
| CompanyName | nvarchar(40) | Not Null |
| Phone | nvarchar(24) | Null |

## Table Suppliers
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| SupplierID | int | **PK**, Identity (1,1), Not Null |
| CompanyName | nvarchar(40) | Not Null |
| ContactName | nvarchar(30) | Null |
| ContactTitle | nvarchar(30) | Null |
| Address | nvarchar(60) | Null |
| City | nvarchar(15) | Null |
| Region | nvarchar(15) | Null |
| PostalCode | nvarchar(10) | Null |
| Country | nvarchar(15) | Null |
| Phone | nvarchar(24) | Null |
| Fax | nvarchar(24) | Null |
| HomePage | ntext | Null |

## Table Orders
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| OrderID | int | **PK**, Identity (1,1), Not Null |
| CustomerID | nchar(5) | FK → **Customers**(CustomerID) |
| EmployeeID | int | FK → **Employees**(EmployeeID) |
| OrderDate | datetime | Null |
| RequiredDate | datetime | Null |
| ShippedDate | datetime | Null |
| ShipVia | int | FK → **Shippers**(ShipperID) |
| Freight | money | Default(0) |
| ShipName | nvarchar(40) | Null |
| ShipAddress | nvarchar(60) | Null |
| ShipCity | nvarchar(15) | Null |
| ShipRegion | nvarchar(15) | Null |
| ShipPostalCode | nvarchar(10) | Null |
| ShipCountry | nvarchar(15) | Null |

## Table Products
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| ProductID | int | **PK**, Identity (1,1), Not Null |
| ProductName | nvarchar(40) | Not Null |
| SupplierID | int | FK → **Suppliers**(SupplierID) |
| CategoryID | int | FK → **Categories**(CategoryID) |
| QuantityPerUnit | nvarchar(20) | Null |
| UnitPrice | money | Default(0), **CK_Products_UnitPrice** (≥ 0) |
| UnitsInStock | smallint | Default(0), **CK_UnitsInStock** (≥ 0) |
| UnitsOnOrder | smallint | Default(0), **CK_UnitsOnOrder** (≥ 0) |
| ReorderLevel | smallint | Default(0), **CK_ReorderLevel** (≥ 0) |
| Discontinued | bit | Default(0), Not Null |

## Table Order Details
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| OrderID | int | **PK**, FK → **Orders**(OrderID) |
| ProductID | int | **PK**, FK → **Products**(ProductID) |
| UnitPrice | money | Not Null, Default(0), **CK_UnitPrice** (≥ 0) |
| Quantity | smallint | Not Null, Default(1), **CK_Quantity** (> 0) |
| Discount | real | Not Null, Default(0), **CK_Discount** (0 ≤ Discount ≤ 1) |

## Table CustomerCustomerDemo
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| CustomerID | nchar(5) | **PK**, FK → **Customers**(CustomerID) |
| CustomerTypeID | nchar(10) | **PK**, FK → **CustomerDemographics**(CustomerTypeID) |

## Table CustomerDemographics
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| CustomerTypeID | nchar(10) | **PK**, Not Null |
| CustomerDesc | ntext | Null |

## Table Region
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| RegionID | int | **PK**, Not Null |
| RegionDescription | nchar(50) | Not Null |

## Table Territories
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| TerritoryID | nvarchar(20) | **PK**, Not Null |
| TerritoryDescription | nchar(50) | Not Null |
| RegionID | int | FK → **Region**(RegionID) |

## Table EmployeeTerritories
| Columna | Tipo | Restricciones |
| --- | --- | --- |
| EmployeeID | int | **PK**, FK → **Employees**(EmployeeID) |
| TerritoryID | nvarchar(20) | **PK**, FK → **Territories**(TerritoryID) |
