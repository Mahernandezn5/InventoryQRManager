# Inventory QR Manager

Sistema de gestión de inventario con códigos QR desarrollado en .NET 6 y Windows Forms.

## Características

- ✅ Gestión completa de inventario
- ✅ Generación automática de códigos QR
- ✅ Base de datos SQLite integrada
- ✅ Interfaz gráfica intuitiva
- ✅ Búsqueda y filtrado de items
- ✅ Categorización y ubicación de productos

## Requisitos del Sistema

- Windows 10 o superior
- .NET 6.0 Runtime
- Visual Studio 2022 (para desarrollo)

## Instalación

1. Clona o descarga el proyecto
2. Abre `InventoryQRManager.sln` en Visual Studio
3. Restaura los paquetes NuGet
4. Compila y ejecuta el proyecto

## Uso

### Primera Ejecución
1. Al ejecutar la aplicación por primera vez, se creará automáticamente la base de datos SQLite
2. Se insertarán categorías y ubicaciones por defecto

### Gestión de Inventario
1. **Agregar Item**: Usa el botón "Nuevo" o el menú "Archivo > Nuevo Item"
2. **Editar Item**: Doble clic en un item o selecciónalo y usa "Editar"
3. **Eliminar Item**: Selecciona un item y usa "Eliminar"
4. **Generar QR**: Selecciona un item y usa "Generar QR"

### Campos del Item
- **Nombre**: Nombre del producto
- **Descripción**: Descripción detallada
- **SKU**: Código único del producto
- **Cantidad**: Stock disponible
- **Precio**: Precio unitario
- **Categoría**: Categoría del producto
- **Ubicación**: Ubicación física
- **Código QR**: Generado automáticamente

## Estructura del Proyecto

```
InventoryQRManager/
├── Data/                 # Capa de datos
│   └── DatabaseContext.cs
├── Models/               # Modelos de datos
│   ├── InventoryItem.cs
│   ├── Category.cs
│   └── Location.cs
├── Services/             # Servicios de negocio
│   ├── InventoryService.cs
│   └── QRCodeService.cs
├── Views/                # Interfaces de usuario
│   ├── MainForm.cs
│   └── AddEditItemForm.cs
├── Program.cs            # Punto de entrada
├── InventoryQRManager.csproj
└── InventoryQRManager.sln
```

## Tecnologías Utilizadas

- **.NET 6.0**: Framework principal
- **Windows Forms**: Interfaz gráfica
- **SQLite**: Base de datos
- **ZXing.Net**: Generación y lectura de códigos QR
- **System.Drawing**: Manipulación de imágenes

## Funcionalidades Implementadas

### ✅ Completadas
- [x] Estructura básica del proyecto
- [x] Modelos de datos
- [x] Base de datos SQLite
- [x] Servicios de inventario
- [x] Generación de códigos QR
- [x] Interfaz principal
- [x] Formulario de agregar/editar items
- [x] CRUD completo de inventario

### 🚧 En Desarrollo
- [ ] Escaneo de códigos QR
- [ ] Impresión de etiquetas
- [ ] Importación/exportación de datos
- [ ] Búsqueda avanzada
- [ ] Reportes y estadísticas
- [ ] Backup y restauración

## Base de Datos

La aplicación utiliza SQLite como base de datos local. El archivo `inventory.db` se crea automáticamente en el directorio de la aplicación.

### Tablas
- **InventoryItems**: Items del inventario
- **Categories**: Categorías de productos
- **Locations**: Ubicaciones físicas

## Códigos QR

Los códigos QR se generan automáticamente con el formato:
```
INV-{SKU}-{Nombre}-{Timestamp}
```

Esto garantiza la unicidad de cada código QR.

## Desarrollo

### Agregar Nuevas Funcionalidades
1. Crea los modelos necesarios en `Models/`
2. Implementa la lógica en `Services/`
3. Crea las interfaces en `Views/`
4. Actualiza la base de datos si es necesario

### Compilación
```bash
dotnet build
```

### Ejecución
```bash
dotnet run
```

## Contribución

1. Fork el proyecto
2. Crea una rama para tu feature
3. Commit tus cambios
4. Push a la rama
5. Abre un Pull Request

## Licencia

Este proyecto está bajo la Licencia MIT.

## Soporte

Para soporte técnico o reportar bugs, por favor abre un issue en el repositorio.

---

**Versión**: 1.0.0  
**Última actualización**: 2024

