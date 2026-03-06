[**KeyStock Frontend v0.1.0**](../../../README.md)

***

[KeyStock Frontend](../../../modules.md) / [pages/MovimientosPage](../README.md) / default

# Function: default()

> **default**(): `Element`

Defined in: [pages/MovimientosPage.js:28](https://github.com/FernandoM-99/Sistema-inventario-frontend/blob/ce3784b6f2d5c583cf800429296803664a50b4fd/src/pages/MovimientosPage.js#L28)

## Returns

`Element`

Interfaz de la bitácora de movimientos y su formulario de registro.

## Component

MovimientosPage

## Description

Página para visualizar y registrar los movimientos de inventario (entradas, salidas, ajustes).
Consume múltiples endpoints para poblar los selectores del formulario (Productos, Usuarios, Proveedores).
Delega en la API (backend) la validación de stock insuficiente mediante el código de respuesta HTTP 400.
