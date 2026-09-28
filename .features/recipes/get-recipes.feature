Feature: Get recipes
    As a user I want to get all recipes
    so that I can browse the cookbook

    Scenario: Get list of recipes
        Given There are 3 recipes in the database
        When I am on the recipes page
        Then I should see the recipes with title, description, cooking time, difficulty, photo, tag and rating

    Examples:
       | Title              | Description           | Cooking time | Difficulty | Photo         | Tag     | Rating |
       | Pasta Carbonara    | Classic Italian pasta | 30           | Easy       | carbonara.jpg | dinner  | 4.5    |
       | Chicken Stir Fry   | Quick Asian dish      | 25           | Medium     | stirfry.jpg   | quick   | 4.0    |
       | Chocolate Cake     | Rich dessert          | 60           | Hard       | cake.jpg      | dessert | 5.0    |
