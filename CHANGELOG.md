# Changelog

## 1.2.0
### New Ingredients
- Added Fertilizer — available at both Gas-Mart shops from Hoodlum I
- Added Hot Sauce — available at both Gas-Mart shops from Peddler III

### Fertilizer Effects
- Added Earthy base effect
- Added Root Bound effect (Earthy + Calming)
- Added Overgrown effect (Root Bound + Toxic) — green screen overlay, NPCs move sluggishly
- Added Bloom effect (Overgrown + Energizing) — flower petal particles, golden screen tint
- Added Compost effect (Bloom + Foggy) — blackout flash on apply, NPCs stumble in confusion

### Hot Sauce Effects
- Added Spicy base effect
- Added Five Alarm effect (Spicy + Toxic)
- Added Capsaicin Rush effect (Five Alarm + Energizing) — smoke breath on player and NPCs
- Added Dragon Breath effect (Capsaicin Rush + Sneaky) — fire breath on player and NPCs
- Added Inferno effect (Dragon Breath + Sedating) — full body fire with smoke and point light glow

### New Lipstick Effects
- Added Femme Fatale effect (Voluptuous + Foggy) — pink/gold sparkle particles, soft pink screen tint
- Added Venom Kiss effect (Diva + Toxic) — deep purple-green screen tint

### New Airhorn Effects
- Added Thunderclap effect (Airhorn + Toxic) — blue-white screen flash on apply, persistent electric-blue tint
- Added White Noise effect (Fiesta + Sedating) — grey desaturation overlay, NPCs move sluggishly

### New Magic Juice Effects
- Added Phantasm effect (Magic + Toxic) — blinding white screen flash; no stat change, purely experiential

### Cross-Ingredient Effects
- Added Wildflower effect (Earthy + Voluptuous / Fertilizer + Lipstick) — flower petals, green-gold screen tint
- Added Burning Passion effect (Voluptuous + Spicy / Lipstick + Hot Sauce) — pink fire particles, pink screen tint
- Added Upheaval effect (Earthy + Airhorn / Fertilizer + Airhorn) — dust flash on apply, earthy screen tint

### Bug Fixes
- Fixed base-game effect combinations (e.g. thought-provoking + horse semen → electrifying) being bypassed by the custom mixing patch
- Fixed all custom effect combinations so mixing order no longer matters — both directions produce the same result

## 1.1.0
### New Features
- Added Moon Dust, Lipstick, and Airhorn to both Gas-Mart shops (Central and West)
- Moon Dust unlocks at Hustler V, Lipstick at Bagman III, Airhorn at Enforcer I
- All three ingredients are available for delivery via the phone delivery app
- Added The Alchemist — a secret vendor stand located in the backyard of an unused building
- The Alchemist sells Magic Juice for $200, unlocked at Enforcer IV
- The Alchemist appears on the map once you reach Enforcer IV
- Upon reaching Enforcer IV, you receive a phone message from The Alchemist directing you to the map
- The Alchemist stand features a glowing purple orb and solid collision

## 1.0.0
### New Ingredients
- Added Moon Dust ingredient
- Added Lipstick ingredient
- Added Airhorn ingredient
- Added Magic Juice ingredient

### Moon Dust Effects
- Added Helium Infusion effect
- Added Moon Gravity effect
- Added Void effect (Moon Gravity + Foggy)
- Added Alien effect (Moon Gravity + Toxic)
- Added Martian effect (Moon Gravity + Moon Gravity)
- Added Super Martian effect (Alien + Martian)
- Added Bunny effect (Helium Infusion + Energizing)
- Added Nebula effect (Helium Infusion + Sedating)
- Added Phantom effect (Helium Infusion + Sneaky)
- Added Wraith effect (Phantom + Helium Infusion)
- Added Tall effect (Helium Infusion + Giraffying)
- Added Inversion effect (Tall + Foggy)

### Lipstick Effects
- Added Voluptuous effect — enhanced body proportions, dramatic eyes, lip overlay, eyelashes, 15 minute duration
- Added Gold Rush effect (Voluptuous + Energizing) — gold skin, gold lips, platinum hair, 15 minute duration
- Added Siren effect (Voluptuous + Calming) — rose gold skin, speed boost, 15 minute duration
- Added Diva effect (Voluptuous + Energizing chain) — platinum hair, glowing eyes, 15 minute duration
- Added Single Event Upset effect (Voluptuous + Void) — fake BSOD screen for 10 seconds
- Added Charged effect (Voluptuous + Electrifying) — zap particles, yellow eyes, speed boost
- Added Gilded Martian effect (Gold Rush + Martian) — cycling gold/orange skin

### Airhorn Effects
- Added Airhorn base effect — plays airhorn sound on consumption
- Added Troll effect (Airhorn + Sneaky) — hidden 5 minute timer then knock sound
- Added Fiesta effect (Airhorn + Energizing) — party sound, confetti, speed boost
- Added Creepy effect (Troll + Airhorn) — plays suspenseful music

### Magic Juice Effects
- Added Magic base effect
- Added Super Magic effect (Magic + Magic) — cycling purple/gold skin
- Added Make It Rain effect (Magic + Calming) — triggers rain storm [CURRENTLY NOT WORKING]
- Added Super Charge effect (Magic + Energizing) — 2.5x speed, 3x jump, 10 minute duration
- Added Supercharge effect (Super Charge + Super Magic) — same as Super Charge, 30 minute duration
- Added Profit Boost effect (Super Magic + Slippery) — +15% profit on all sales, 10 minute duration

### Framework
- Added OOP ingredient and effect framework for easy expansion
- Added CustomIngredient base class
- Added CustomEffect base class
- Added EffectCombination system for two-effect combinations
- Added MultiEffectCombination system for three-effect combinations
- Added CustomMixRegistry for registering all custom combinations
- Added chain reaction system — combinations can trigger further combinations automatically
- Added WAV audio loader utility
- Added texture cache utility
- Added SoundHelper utility for respecting game volume settings
- Added Stability patches for mixing station UI fixes
