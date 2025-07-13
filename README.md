# Turf War - Unity Implementation

A comprehensive implementation of the "Turf War" tactical card game in Unity, following the step-by-step development guide for junior Unity developers.

## Overview

"Turf War" is a turn-based tactical card game where players compete to capture secretly assigned turfs on a hexagonal board. The game features:

- **Hexagonal Battlefield**: 36-hex board with procedural generation and "stair" visual effects
- **Card System**: Deploy cards (pastel colors) and Event cards (vibrant colors) with unique abilities
- **Token System**: Four levels of deployable tokens with capture mechanics
- **Leader System**: Passive and active abilities that define player strategy
- **Reaction Window**: Unique 10-second reaction phase for counter-play
- **Turn-Based Gameplay**: Structured turns with action constraints and timers

## Project Structure

### Core Systems

#### Data Management (ScriptableObjects)
- `BaseGameDataSO.cs` - Base class for all game data assets
- `CardDataSO.cs` - Defines card properties and abilities
- `TokenDataSO.cs` - Defines token characteristics and levels
- `LeaderDataSO.cs` - Defines leader abilities and properties
- `AbilityDataSO.cs` - Generic ability system for effects

#### Game Management
- `GameManager.cs` - Central game state management using Finite State Machine
- `TurnManager.cs` - Turn flow and action constraint management
- `CardManager.cs` - Card instantiation and hand management
- `GameTimer.cs` - Overall game countdown timer

#### Hexagonal Grid System
- `Hex.cs` - Axial coordinate system for hexagonal grid
- `HexTile.cs` - Individual tile representation with ownership tracking
- `HexGridGenerator.cs` - Procedural board generation with "stair" effects

#### Gameplay Controllers
- `PlayerController.cs` - Player action handling and card execution
- `TokenController.cs` - Token behavior and status effects
- `LeaderController.cs` - Leader ability management
- `ReactionHandler.cs` - Reaction window implementation

#### Input and UI
- `HexInputHandler.cs` - Mouse/touch input for hex tile selection
- `CardDisplay.cs` - Visual card representation and interaction
- `UIPanelController.cs` - Basic UI show/hide functionality

## Setup Instructions

### Prerequisites
- Unity 2022.3 LTS or later
- TextMeshPro package (for UI text rendering)

### Installation

1. **Clone or download the project**
2. **Open in Unity Hub**
3. **Set up the scene structure**:
   - Create an empty GameObject named "GameManager" and attach the `GameManager.cs` script
   - Create an empty GameObject named "HexGridGenerator" and attach the `HexGridGenerator.cs` script
   - Create an empty GameObject named "CardManager" and attach the `CardManager.cs` script
   - Create an empty GameObject named "TurnManager" and attach the `TurnManager.cs` script
   - Create an empty GameObject named "GameTimer" and attach the `GameTimer.cs` script
   - Create an empty GameObject named "PlayerController" and attach the `PlayerController.cs` script
   - Create an empty GameObject named "ReactionHandler" and attach the `ReactionHandler.cs` script
   - Create an empty GameObject named "HexInputHandler" and attach the `HexInputHandler.cs` script

4. **Create prefabs**:
   - **HexTile Prefab**: Create a 3D hexagon model with `HexTile.cs` script attached
   - **Card Prefab**: Create a UI card prefab with `CardDisplay.cs` script attached
   - **Token Prefabs**: Create 3D token models with `TokenController.cs` script attached

5. **Set up UI Canvas**:
   - Create a Canvas with UI elements for:
     - Player hand area (Horizontal Layout Group)
     - Game timer display
     - Reaction window panel
     - Turn timer display

6. **Configure ScriptableObjects**:
   - Create card data assets using `CardDataSO`
   - Create token data assets using `TokenDataSO`
   - Create leader data assets using `LeaderDataSO`
   - Create ability data assets using `AbilityDataSO`

7. **Assign references in Inspector**:
   - Link all managers to their required references
   - Assign prefabs to their respective managers
   - Set up UI element references

## Game Rules Implementation

### Turn Structure
- Players draw 1 card at the start of their turn
- Players can play exactly 1 Deploy card per turn
- Players can play unlimited Event cards per turn
- Players can use Leader abilities (with cooldowns)
- Turn time limit: 60 seconds (configurable)

### Card Types
- **Deploy Cards**: Place tokens on the board (pastel colors)
- **Event Cards**: Trigger special effects (vibrant colors)

### Token System
- **Level 1-4**: Higher levels can capture lower levels
- **Level 3**: Special units with unique abilities
- **Level 4 (Alpha)**: Single strongest token per player

### Reaction Window
- 10-second window for opponent to respond to actions
- Can play Event cards or dismiss
- Auto-dismisses if no action taken

### Victory Conditions
- Primary: Capture more of your 3 secretly assigned turfs
- Tiebreaker: Capture more total turfs on the board
- Time limit: 15-20 minutes (configurable)

## Key Features

### Modular Architecture
- **Event-Driven Design**: Loose coupling between systems using C# events
- **Data-Driven**: ScriptableObjects for easy content creation and balancing
- **State Machine**: Clear game state management with FSM pattern

### Extensible Systems
- **Ability System**: Generic framework for card and leader effects
- **Hex Grid**: Reusable hexagonal coordinate system
- **UI Framework**: Modular UI components for easy expansion

### Performance Optimizations
- **Object Pooling**: Efficient instantiation of cards and tokens
- **Event System**: Minimal coupling between game systems
- **Memory Management**: Proper cleanup of event subscriptions

## Development Roadmap

### Phase 1: Core Prototype ✅
- [x] Basic hexagonal grid system
- [x] Card and token data structures
- [x] Turn-based gameplay loop
- [x] Basic UI framework

### Phase 2: Enhanced Gameplay
- [ ] Advanced ability system implementation
- [ ] AI opponent development
- [ ] Visual effects and animations
- [ ] Sound system integration

### Phase 3: Multiplayer Foundation
- [ ] Networking architecture design
- [ ] Client-server communication
- [ ] State synchronization
- [ ] Anti-cheat measures

### Phase 4: Live Service Features
- [ ] Card collection system
- [ ] Deck building interface
- [ ] Guild system
- [ ] Monetization framework

## Best Practices Implemented

### Code Organization
- **Separation of Concerns**: Clear division between data, logic, and presentation
- **Single Responsibility**: Each script has a focused purpose
- **Dependency Injection**: Manager references assigned through Inspector

### Unity-Specific Patterns
- **ScriptableObjects**: For data persistence and designer-friendly content creation
- **Events**: For loose coupling between systems
- **Coroutines**: For time-based operations without blocking
- **Singleton Pattern**: For global game state management

### Performance Considerations
- **Object Pooling**: For frequently instantiated objects
- **Event Cleanup**: Proper unsubscription to prevent memory leaks
- **Efficient Data Structures**: Dictionary lookups for hex tile access

## Troubleshooting

### Common Issues

1. **Hex tiles not generating**: Check that HexTile prefab is assigned to HexGridGenerator
2. **Cards not displaying**: Ensure CardDisplay script is attached to card prefab
3. **Input not working**: Verify HexInputHandler has correct layer mask set
4. **Game state not updating**: Check that all managers are properly referenced

### Debug Features
- Console logging for all major game events
- Inspector-exposed properties for easy monitoring
- Context menu options for testing (e.g., "Generate Grid")

## Contributing

This implementation serves as a foundation for the "Turf War" game. Key areas for contribution:

1. **Content Creation**: Design new cards, tokens, and abilities using ScriptableObjects
2. **UI Enhancement**: Improve visual presentation and user experience
3. **Gameplay Balance**: Fine-tune mechanics and timing
4. **Performance Optimization**: Identify and resolve bottlenecks
5. **Feature Expansion**: Implement additional game modes and systems

## License

This implementation is provided as a learning resource for Unity development. Feel free to use, modify, and extend for educational and development purposes.

---

**Note**: This is a prototype implementation. For production use, additional considerations include networking, security, optimization, and comprehensive testing. 