# ChuanLeMaServer
### 迁移数据库 cmd切换到 repository目录 执行
```
set ConnectionStrings__DefaultConnection=Server=localhost;Port=3306;Database=chuanlema;User=root;Password=1;
dotnet ef migrations add InitialCreate
dotnet ef database update
```