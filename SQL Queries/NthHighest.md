Nth highest salary 
Option 1 : Using a Common Table Expression (CTE) with ROW_NUMBER()
WITH SalaryRank AS (
    SELECT DISTINCT Salary,
           ROW_NUMBER() OVER (ORDER BY Salary DESC) AS Rank
    FROM Employees
)
SELECT Salary
FROM SalaryRank
WHERE Rank = N;
Option 2 : Using DENSE_RANK()
WITH SalaryRank AS (
    SELECT DISTINCT Salary,
           DENSE_RANK() OVER (ORDER BY Salary DESC) AS Rank
    FROM Employees
)
SELECT Salary
FROM SalaryRank
WHERE Rank = N;
