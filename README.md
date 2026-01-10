<img src="https://github.com/EnesEfeTokta/Atlas/blob/main/Atlas/Assets/Logo/Logo.png" style="width:150px;" />

# ATLAS

**Atlas** is a real-time multiplayer strategy game developed with Unity, set in the blue lands of planet Atlas. Two players engage in strategic combat by building defense towers and deploying modern war vehicles. Players must tactically use both defensive structures and various ground and air units to capture their opponent's base. Quick thinking, resource management, and masterful tower placement skills are critical for victory!

## Features
- **Multiplayer Strategy Combat:** Intense 1v1 matches where two players face off in real-time.
- **Tower Defense System:** Protect your territory and repel enemy attacks by building defense towers at strategic points.
- **Diverse War Vehicles:** Build your army with tanks, air units, and light vehicles, and plan your attacks.
- **Atlas Battlefield:** Fight on a dynamic map filled with craters, hills, and strategic points on the blue planet.
- **Resource and Energy Management:** Empower your units by building energy towers and maintain continuous production with your factories.
- **Strategic Depth:** Defeat your opponent by utilizing each unit's unique strengths and weaknesses.

## Screenshots
![Main Menu](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/MainMenu.png)
![Main Menu Settings 1](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/MainMenuSettings1.png)
![Main Menu Settings 2](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/MainMenuSettings2.png)
![Planet](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/Planet.png)
![Area](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/Area.png)
![GameScene 1](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/GameScene1.png)
![GameScene 2](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/GameScene2.png)
![GameScene 3](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/GameScene3.png)
![GameScene 4](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/GameScene4.png)
![GameScene 5](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/GameScene5.png)
![GameScene 6](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/GameScene6.png)
![GameScene 7](https://github.com/EnesEfeTokta/Atlas/blob/main/Screenshots/GameScene7.png)

## Installation
Follow these steps to run ATLAS on your local machine:

### Requirements
- Unity 6 or higher
- Git/GitHub
- Photon Fusion (for multiplayer, optional)

### Steps
1. Clone this repository:
   ```bash
   git clone https://github.com/username/red-strike.git
   ```
2. Open Unity Hub and add the project using the "Add" button.
3. Open the project in Unity Editor.
4. Download required packages (e.g., Photon Fusion) from Unity Package Manager.
5. Open the main scene from the "Scenes" folder and press the "Play" button!

## Gameplay
**Objective:** Destroy your opponent's main base with strategic tower defense and attack units!

**Game Mechanics:**
- **Tower Construction:** Place defensive and offensive towers at strategic points on the map.
- **Unit Production:** Use your factories to produce tanks, air vehicles, and light units.
- **Energy Management:** Build energy towers to ensure your vehicles operate at full power.
- **Tactical Attack:** Coordinate your ground and air units to break through enemy defenses.

**Strategy Tips:**
- Build energy towers first - your units will work more efficiently.
- Place towers on high ground to gain range advantage.
- Discover your opponent's weak points and direct your attacks there.
- Surprise your opponent by using both air and ground units in a balanced way.
- Protect your factories - continuous production is critical for victory!

## In-Game Elements

### Defense and Production Structures

---

**Main Base** (Main headquarters. Its destruction means losing the game.)
   - **Health:** 800 hp
   - **Range:** None
   - **Density:** 1 (Single instance)
   - **Respawn:** No
   - **Feature:** The fundamental structure of the game. Strategic tower placement is essential for protection.
  
**Factory** (The center where all war vehicles are produced.)
   - **Health:** 400 hp
   - **Range:** 10 units
   - **Density:** 2
   - **Respawn:** Yes (90 seconds)
   - **Production Capacity:** Maximum 2 units can be produced simultaneously
   - **Feature:** If destroyed, new unit production stops. Must protect!

**Energy Tower** (Provides necessary energy for vehicles to operate at maximum performance.)
   - **Health:** 300 hp
   - **Density:** 3
   - **Respawn:** Yes (50 seconds)
   - **Energy Transfer Capacity:** Energy can be transferred to maximum 2 vehicles simultaneously
   - **Special Ability:** Without energy, vehicles operate 50% slower
   - **Strategy:** Should be built early game. More energy = Stronger army!

### Air Units

---

**Ornithopter A** (Fast but low durability.)  
- **Health:** 120 hp 
- **Damage:** 40 dmg
- **Speed:** 300
- **Energy:** 500 lt
- **Fire Rate:** 1 second
- **Range:** 8 units
- **Density:** 5
- **Respawn:** Yes (10 seconds)
- **Production Cost:** Medium  
- **Special Ability:** Takes 20% less damage while moving.  

**Ornithopter B** (More durable but slower.)  
- **Health:** 250 hp
- **Damage:** 80 dmg 
- **Speed:** 100
- **Energy:** 400 lt
- **Fire Rate:** 3 seconds
- **Range:** 10 units
- **Density:** 4  
- **Respawn:** Yes (20 seconds)
- **Production Cost:** High
- **Special Ability:** Shield Active: Takes 50% less damage for the first 2 seconds.

### Ground Units

---

**Tank Heavy A** (The most powerful tank in the game.)  
- **Health:** 300 hp
- **Damage:** 220 dmg
- **Speed:** 20
- **Energy:** 500 lt
- **Fire Rate:** 10 seconds
- **Range:** 15 units
- **Density:** 2
- **Respawn:** Yes (40 seconds) 
- **Production Cost:** Very High
- **Special Ability:** Armor Piercing: Penetrates enemy armor by 20%. 

**Tank Heavy B** (Lighter than A version but still powerful.)  
- **Health:** 220 hp
- **Damage:** 160 dmg 
- **Speed:** 40
- **Energy:** 400 lt
- **Fire Rate:** 7 seconds
- **Range:** 12 units
- **Density:** 3
- **Respawn:** Yes (30 seconds)
- **Production Cost:** High
- **Special Ability:** Low Fuel Mode: Moves 30% faster when health drops below 50%.

**Tank Combat** (A faster tank.)  
- **Health:** 180 hp
- **Damage:** 120 dmg
- **Speed:** 120
- **Energy:** 300 lt
- **Fire Rate:** 4 seconds  
- **Range:** 10 units
- **Density:** 5
- **Respawn:** Yes (15 seconds) 
- **Production Cost:** Medium
- **Special Ability:** Surprise Attack: Deals 25% extra damage on its first shot.

### Light and Agile Units

---

**Quat** (Agile and flexible unit.)  
- **Health:** 140 hp
- **Damage:** 60 dmg
- **Speed:** 150
- **Energy:** 500 lt
- **Fire Rate:** 4 seconds
- **Range:** 6 units
- **Density:** 5
- **Respawn:** Yes (10 seconds)
- **Production Cost:** Low
- **Special Ability:** Evasion: Takes 15% less damage while moving.

**Infantry Light** (The fastest ground unit.)  
- **Health:** 120 hp
- **Damage:** 60 dmg
- **Speed:** 250
- **Energy:** 400 lt
- **Fire Rate:** 2 seconds
- **Range:** 4 units
- **Density:** 5
- **Respawn:** Yes (5 seconds)  
- **Production Cost:** Very Low
- **Special Ability:** Ambush: Deals 50% more damage on the first shot when attacking an enemy.

**Trike** (A balanced ground vehicle.)  
- **Health:** 150 hp
- **Damage:** 70 dmg
- **Speed:** 120
- **Energy:** 300 lt
- **Fire Rate:** 4 seconds
- **Range:** 8 units
- **Density:** 5
- **Respawn:** Yes (5 seconds)
- **Production Cost:** Medium 
- **Special Ability:** Double Shot: 10% chance to fire twice.
