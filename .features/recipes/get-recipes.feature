Feature: Get recipes
    As a user I want to get all recipes
    so that I can browse the cookbook

    Scenario: Get list of recipes
        Given There are 3 recipes in the database
        When I am on the recipes page
        Then I should see the recipes with title, description, cooking time, difficulty and photo

    Examples:
       | Title              | Description           | Cooking time | Difficulty | Photo        |
       | Pasta Carbonara    | Classic Italian pasta | 30           | Easy       | carbonara.jpg|
       | Chicken Stir Fry   | Quick Asian dish      | 25           | Medium     | stirfry.jpg  |
       | Chocolate Cake     | Rich dessert          | 60           | Hard       | cake.jpg     |
