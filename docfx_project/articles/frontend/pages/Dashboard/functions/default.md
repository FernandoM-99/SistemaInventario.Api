[**KeyStock Frontend v0.1.0**](../../../README.md)

***

[KeyStock Frontend](../../../modules.md) / [pages/Dashboard](../README.md) / default

# Function: default()

> **default**(): `Element`

Defined in: [pages/Dashboard.js:13](https://github.com/FernandoM-99/Sistema-inventario-frontend/blob/ce3784b6f2d5c583cf800429296803664a50b4fd/src/pages/Dashboard.js#L13)

## Returns

`Element`

Panel de control con tarjetas de indicadores.

## Component

Dashboard

## Description

Vista principal que muestra los KPIs del sistema: total de productos en stock, 
cantidad de movimientos realizados en el día actual y total de proveedores activos.
Utiliza peticiones asíncronas simultáneas (Promise.all) para optimizar la carga de datos.
