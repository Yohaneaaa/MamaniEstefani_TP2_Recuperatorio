# Reality

**Recuperatorio TP N°2 – Programación de Videojuegos 1 (TUDIVJ, Facultad de Ingeniería, UNJu)**

## Datos del estudiante
- **Nombre:** Mamani Estefani Gabriela
- **Legajo tuv000820 / DNI:48530450 
- **Usuario de GitHub:** Yohaneaaa
- **Versión de Unity:** 6.3 LTS (6000.3.22f1), Universal Render Pipeline

## Descripción
Reality es un prototipo 3D ambientado en un terreno abierto al anochecer. El jugador está atrapado en una realidad que no es la suya: unos monstruos lo persiguen y la única salida es reunir los fragmentos de realidad repartidos por el mapa y activar la terminal.

**Objetivo:** juntar 3 fragmentos (30 puntos), llegar a la terminal y activarla con la tecla E, sin que los monstruos acaben con la vida del jugador.

## Controles
| Tecla | Acción |
|---|---|
| W A S D | Mover al personaje (relativo a la cámara) |
| Espacio | Saltar (solo sobre el suelo) |
| Flechas | Girar y subir o bajar la cámara |
| E | Interactuar con la terminal |
| Enter | Comenzar el juego |
| Esc o P | Pausar y reanudar |
| R | Reiniciar (en Victoria o Derrota) |

## Requisitos obligatorios

### 1. Raycast y LayerMask
- **Chequeo de suelo** (`PlayerMovement`): un rayo desde los pies hacia abajo, filtrado por la capa `Ground` e ignorando triggers, valida cuándo se puede saltar.
- **Interacción a distancia** (`PlayerInteraction`): un Raycast frontal de 3 m filtrado por la capa `Interactable` detecta objetos que implementan `IInteractable` (la terminal). Con E se activa.
- **Sin autodaño:** las máscaras excluyen la capa del jugador, por lo que los rayos nunca lo golpean.

### 2. NavMesh e IA
- Malla de navegación horneada sobre el terreno con `NavMeshSurface` (Include Layers: `Ground`).
- `SentinelAI` (el monstruo) usa un `NavMeshAgent`: **patrulla** puntos aleatorios de su zona, **detecta** al jugador por distancia (20 m), cono de visión (160°) y línea de vista con Raycast, lo **persigue** y lo **ataca** dentro del rango (1,8 m) o al contacto. Pierde al jugador a 28 m.

### 3. Salud, daño y puntuación
- `PlayerHealth`: 100 HP, con **inmunidad de 1 segundo** tras cada golpe, para que el contacto no reste toda la vida en un frame.
- `DataItem`: los fragmentos suman 10 puntos al tocarlos.

### 4. GameManager
- Singleton con los estados **Start, Playing, Paused, Victory y GameOver**.
- La pausa congela el tiempo con `Time.timeScale = 0`.
- Escucha los eventos de `PlayerHealth` (el jugador no tiene lógica de juego) e informa por consola los cambios de estado, la vida y el puntaje.

### 5. Terreno
Escenario modelado con **Terrain Tool**: desniveles, rampas y texturas con Terrain Layers, más árboles y rocas.

## Estación de Curación (requisito extra)
Plataforma cilíndrica con luz y partículas, con un collider **trigger** que detecta al jugador (`HealingStation`).
- **Curación progresiva:** si el jugador está herido y dentro de la zona, recibe **+10 HP cada 1,5 s**.
- **Límite de carga:** la estación entrega como máximo **50 HP** y luego se agota.
- **Cooldown:** entra en recarga durante **8 segundos**, sin curar.
- **Retroalimentación:** durante la recarga se **apagan la luz y las partículas**, el HUD muestra "Estación en recarga" y se informa por consola.

## Requisitos opcionales elegidos
- **Opción B, Power-Up de Invisibilidad** (`InvisibilityPowerUp`): al recogerlo, el jugador es indetectable para la IA durante 8 segundos. El jugador cambia de color y el HUD muestra la cuenta regresiva.
- **Opción D, Zona de Barro** (`MudZone`): trigger que reduce la velocidad al 40% mientras el jugador está dentro y la restablece al salir.
- **Opción E, Spawner** (`EnemySpawner`): genera un monstruo nuevo cada 10 s, con un máximo de 3 vivos, siempre sobre el NavMesh. No genera en pausa ni en Victoria o Derrota.

## Correcciones aplicadas
- **Interacción por Raycast:** el rayo se filtra solo por la capa `Interactable` y parte un poco por detrás y más bajo que el centro del jugador, para funcionar aunque esté pegado al objeto y para detectar objetos bajos.
- **Capas:** `Ground` e `Interactable` con los nombres exactos que usan los scripts.
- **Visión del monstruo:** el rayo de línea de vista ignora al propio monstruo, y se amplió el radio y el ángulo de detección.
- **Ataque por rango:** además del daño por contacto, el monstruo ataca si el jugador entra en su rango. Ambos pasan por el cooldown de `PlayerHealth`.
- **Salud:** se agregó `Heal()` en `PlayerHealth` para la estación de curación.
- **GameManager desacoplado:** el flujo del juego se controla por eventos, sin cargar de lógica al jugador.
- **Unity 6:** `rb.velocity` se reemplazó por `rb.linearVelocity`.
- **Repositorio nuevo:** proyecto desde cero, con `.gitignore` de Unity y commits progresivos.
- [Completar con las observaciones de la corrección de la primera entrega.]

## Capturas
![Victoria](Capturas/victoria.png)
![Estación de curación](Capturas/estacion.png)
![ZonaBarro](Capturas/ZonaBarro.png)


## Cómo ejecutar
1. Clonar el repositorio.
2. Abrir la carpeta con Unity Hub, con la versión 6.3 LTS (6000.3.22f1).
3. Abrir la escena `Assets/Scenes/SampleScene` y presionar Play.