Feature: Get recipes
    As a user I want to get recipes
    so that I can browse the cookbook according to visibility rules

    Scenario: Guest sees only public recipes
        Given I am not logged in
        And There are recipes in the database
        When I am on the recipes page
        Then I should see only public recipes with title, description, cooking time, difficulty, photo, category, tags, rating and comments count
        And I should not see the favourites mark

    Scenario: Authorized user sees public recipes and own private recipes
        Given I am logged in as "alice"
        And There are recipes in the database
        And I have favourited the recipe "Pasta Carbonara"
        When I am on the recipes page
        Then I should see public recipes and my private recipes with title, description, cooking time, difficulty, photo, category, tags, rating, comments count and favourites mark
        And The favourites mark should be active or inactive according to my favourites

    Examples:
       | Title              | Description           | Cooking time | Difficulty | Photo         | Category | Tags         | Rating | Visibility | Author | Comments count | Favourite |
       | Pasta Carbonara    | Classic Italian pasta | 30           | Easy       | carbonara.jpg | dinner   | quick        | 4.5    | public     | alice  | 3              | active    |
       | Chicken Stir Fry   | Quick Asian dish      | 25           | Medium     | stirfry.jpg   | lunch    | quick, vegan | 4.0    | private    | alice  | 1              | inactive  |
       | Chocolate Cake     | Rich dessert          | 60           | Hard       | cake.jpg      | dessert  |              | 5.0    | private    | bob    | 0              |           |
