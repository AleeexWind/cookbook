# Cookbook

A personal cookbook where people browse, rate, and plan meals around recipes.

## Language

### Recipe catalogue

**Recipe**:
A cookable dish with characteristics, ingredients, steps, and social feedback.
_Avoid_: Dish, meal, food item

**IngredientLine**:
One named ingredient on a recipe with a quantity and unit for the recipe's base portions.
_Avoid_: Ingredient item, product line

**CookingStep**:
An ordered instruction that describes how to prepare the recipe.
_Avoid_: Instruction, direction, stage

**Visibility**:
Whether a recipe is public (visible to everyone) or private (visible only to its author).
_Avoid_: Access level, privacy setting

**Difficulty**:
How hard the recipe is to cook: Easy, Medium, or Hard.
_Avoid_: Complexity, skill level

**Category**:
The meal-time grouping of a recipe (for example breakfast, lunch, dinner, dessert).
_Avoid_: Type, cuisine

**Tag**:
A free-form label used to find recipes (for example quick, vegan).
_Avoid_: Label, keyword

**Portions**:
The number of servings a recipe or menu plan is scaled to.
_Avoid_: Servings, people count

**MeasurementUnit**:
How an ingredient quantity is measured: grams (g), units, or milliliters (ml).
_Avoid_: Unit of measure, UoM

### Social

**Rating**:
A single immutable 1–5 star score a user assigns to a recipe.
_Avoid_: Review score, stars vote

**Comment**:
A timed text note a user leaves on a recipe.
_Avoid_: Review, feedback, note

**Favourite**:
A user's bookmark of a recipe.
_Avoid_: Bookmark, like, star mark

### Menu planning

**MenuPlan**:
One user's week of meal slots with a shared portions value for scaling.
_Avoid_: Meal plan, weekly schedule, calendar

**MealSlot**:
A single day-and-meal position in a menu plan that may hold one recipe.
_Avoid_: Cell, calendar cell, plan entry

**MealType**:
Which meal of the day a slot represents: breakfast, lunch, or dinner.
_Avoid_: Meal kind, course

**ShoppingList**:
An aggregated list of ingredient quantities derived from a menu plan.
_Avoid_: Grocery list, basket, purchase list

**ShoppingListItem**:
One line on a shopping list: ingredient name, total quantity, and unit.
_Avoid_: Grocery line, list entry

### Identity

**User**:
A person who authors recipes, rates, comments, favourites, and owns a menu plan.
_Avoid_: Account, member, profile

**Author**:
The user who created a recipe and may delete comments on it.
_Avoid_: Owner, creator
