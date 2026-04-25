Delete duplicate row from a table 
Option 1 : Using a Temporary Table:
SELECT DISTINCT *
INTO #TempTable
FROM HumanResources.Employee;
TRUNCATE TABLE HumanResources.Employee;
INSERT INTO HumanResources.Employee
SELECT * FROM #TempTable;   DROP TABLE #TempTable;
Option 2 : using Group By and having clause
WITH CTE AS (
    SELECT EmployeeID, Name, JobTitle, COUNT(*) AS cnt
    FROM HumanResources.Employee
    GROUP BY EmployeeID, Name, JobTitle
    HAVING COUNT(*) > 1
)
DELETE FROM HumanResources.Employee
WHERE EmployeeID IN (SELECT EmployeeID FROM CTE);
Option 3: using row_number() and partitioning using order clause
WITH CTE AS (
    SELECT *,
           ROW_NUMBER() OVER (PARTITION BY Name ORDER BY Id) AS rn
    FROM Employee
)
DELETE FROM CTE WHERE rn > 1;
Delete duplicate keeping one original record if the table do not have any primary key
WITH CTE AS (
    SELECT *,
           ROW_NUMBER() OVER (PARTITION BY Name ORDER BY SELECT NULL) AS rn
    FROM Employee
)
DELETE FROM CTE WHERE rn > 1;

