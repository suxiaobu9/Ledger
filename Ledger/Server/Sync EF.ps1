# 吃 appsettings.json 裡面的值，也吃的到 user secrets

dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -o ..\Shared\EF\ --force --no-onconfiguring
pause
