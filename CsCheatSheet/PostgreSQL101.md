## Custom class or struct:

Custom `class` example:

```cs
class MyClass
{
    string nameMaybe;
    int someId;
}
```

PostgreSQL command to define:

```sql
-- only accepts lowerCase
CREATE TYPE my_class AS (
      name_maybe TEXT,
      some_id INT
    );
```

Bind `class` to PostgreSQL Type

```cs
var dataSourceBuilder = new NpgsqlDataSourceBuilder("SQLConnectionHere");
dataSourceBuilder.MapComposite<MyClass>("my_class");
dataSource = dataSourceBuilder.Build();

// also modify your class definition:
class MyClass
{
    [PgName("name_maybe")] //<<--- match the name in the Postgre type definition
    string nameMaybe;
    [PgName("some_id")]
    int someId;
}
```

## PostgreSQL Array

Because Postgre is a chad, it can store arrays.

```pgsql
CREATE TABLE IF NOT EXISTS some_table (
  important_array INT[]
);
```