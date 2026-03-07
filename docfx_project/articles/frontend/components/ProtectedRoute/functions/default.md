[**sistema-inventario-frontend v0.1.0**](../../../README.md)

***

[sistema-inventario-frontend](../../../modules.md) / [components/ProtectedRoute](../README.md) / default

# Function: default()

> **default**(): `Element`

Defined in: [components/ProtectedRoute.js:11](https://github.com/FernandoM-99/Sistema-inventario-frontend/blob/ce3784b6f2d5c583cf800429296803664a50b4fd/src/components/ProtectedRoute.js#L11)

## Returns

`Element`

Renderiza el contenido hijo (`<Outlet />`) si está autenticado, o una redirección.

## Component

ProtectedRoute

## Description

Componente enrutador que protege las rutas privadas de la aplicación.
Verifica el estado de autenticación y redirige a `/login` si el usuario no tiene una sesión activa.
