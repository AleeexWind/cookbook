Feature: Get recipes
    As a user I want to get recipes
    so that I can browse the cookbook according to visibility rules

    Scenario: Guest sees only public recipes
        Given I am not logged in
        And There are recipes in the database
        When I am on the recipes page
        Then I should see only public recipes with title, description, cooking time, difficulty, photo, tag and rating

    Scenario: Authorized user sees public recipes and own private recipes
        Given I am logged in as "alice"
        And There are recipes in the database
        When I am on the recipes page
        Then I should see public recipes and my private recipes with title, description, cooking time, difficulty, photo, tag and rating

    Examples:
       | Title              | Description           | Cooking time | Difficulty | Photo         | Tag     | Rating | Visibility | Author |
       | Pasta Carbonara    | Classic Italian pasta | 30           | Easy       | carbonara.jpg | dinner  | 4.5    | public     | alice  |
       | Chicken Stir Fry   | Quick Asian dish      | 25           | Medium     | stirfry.jpg   | quick   | 4.0    | private    | alice  |
       | Chocolate Cake     | Rich dessert          | 60           | Hard       | cake.jpg      | dessert | 5.0    | private    | bob    |
