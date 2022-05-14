dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -o ..\Shared\EF\ --force --no-onconfiguring

pause