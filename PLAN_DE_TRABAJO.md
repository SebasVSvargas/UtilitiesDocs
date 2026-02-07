# Plan de Trabajo - UtilitiesDocs

Este documento define la hoja de ruta para el desarrollo de la aplicación de escritorio **UtilitiesDocs**.

## 1. Visión del Proyecto
Una aplicación modular para Windows (WinUI 3 / .NET 8) que permite realizar operaciones comunes sobre documentos (principalmente PDF) de forma local, segura y eficiente.

## 2. Arquitectura
- **Framework UI**: Windows App SDK (WinUI 3).
- **Lenguaje**: C# / .NET 8.
- **Patrón de Diseño**: MVVM (Model-View-ViewModel) para separar la lógica de presentación.
- **Librerías Core**:
  - `PdfSharp` (Manejo de PDFs).
  - `CommunityToolkit.Mvvm` (Recomendado para facilitar MVVM).

## 3. Módulos Iniciales (Implementación Prioritaria)
Estructura propuesta de módulos en código:

### 3.1 Unir PDFs (Merge)
- **Funcionalidad**: Seleccionar múltiples archivos PDF y generar un único archivo de salida.
- **Entrada**: Lista de archivos.
- **Proceso**: Concatenación de páginas en orden secuencial.
- **Salida**: Un archivo PDF.

### 3.2 Desbloquear PDF (Remove Password)
- **Funcionalidad**: Procesar archivos PDF protegidos por contraseña para guardar una copia sin protección.
- **Entrada**: Archivos protegidos, contraseña común (o individual).
- **Proceso**: Abrir con clave, guardar con configuración de seguridad nula.
- **Salida**: Archivos PDF libres de contraseña.

## 4. Ideas para Futuros Módulos
Sugerencias para expandir la aplicación:

| Módulo | Descripción | Complejidad |
|--------|-------------|-------------|
| **Dividir PDF** | Extraer páginas específicas o rangos a nuevos archivos. | Baja |
| **PDF a Imágenes** | Convertir cada página de un PDF a JPG/PNG. | Media |
| **Imágenes a PDF** | Crear un PDF a partir de fotos o escaneos. | Baja |
| **Rotar Páginas** | Corregir la orientación de páginas en masa. | Baja |
| **Marcas de Agua** | Agregar texto o logos sobre las páginas (confidencial, borradores, etc.). | Media |
| **Compresión** | Reducir el tamaño del archivo (eliminando metadatos o recomprimiendo imágenes). | Alta |
| **Ordenamiento Visual** | UI para arrastrar y soltar páginas y reorganizarlas antes de guardar. | Media |
| **OCR (Texto)** | Extraer texto de PDFs escaneados (requiere librerías como Tesseract). | Alta |

## 5. Estrategia de Implementación
1. **Configuración Base**: Estructura de carpetas (`Services`, `ViewModels`, `Views`), Inyección de Dependencias.
2. **Servicio PDF Core**: Wrapper sobre `PdfSharp` para operaciones básicas.
3. **UI Módulo Unir**: Vista simple con Drag & Drop.
4. **UI Módulo Desbloquear**: Input de contraseña y lista de archivos.
