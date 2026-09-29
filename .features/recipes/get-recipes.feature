Feature: Get recipes
    As a user I want to get recipes
    so that I can browse the cookbook according to visibility rules

    Scenario: Guest sees only public recipes
        Given I am not logged in
        And There are recipes in the database
        When I am on the recipes page
        Then I should see only public recipes with title, description, cooking time, difficulty, photo, category, tags, average rating (1-5 stars) and comments count
        And I should not see the favourites mark
        And I should see recipes sorted by newest from new to old

    Scenario: Authorized user sees public recipes and own private recipes
        Given I am logged in as "alice"
        And There are recipes in the database
        And I have favourited the recipe "Pasta Carbonara"
        When I am on the recipes page
        Then I should see public recipes and my private recipes with title, description, cooking time, difficulty, photo, category, tags, average rating (1-5 stars), comments count and favourites mark
        And The favourites mark should be active or inactive according to my favourites
        And I should see recipes sorted by newest from new to old

    Scenario: Recipes are sorted by newest by default
        Given I am logged in as "alice"
        And There are recipes in the database
        When I am on the recipes page
        Then I should see recipes sorted by newest from new to old

    Scenario: Sort recipes by average rating descending
        Given I am logged in as "alice"
        And There are recipes in the database
        When I am on the recipes page
        And I sort by average rating desc
        Then I should see recipes sorted by average rating from highest to lowest

    Scenario: Sort recipes by average rating ascending
        Given I am logged in as "alice"
        And There are recipes in the database
        When I am on the recipes page
        And I sort by average rating asc
        Then I should see recipes sorted by average rating from lowest to highest

    Examples:
       | Title              | Description           | Cooking time | Difficulty | Photo         | Category | Tags         | Average rating | Visibility | Author | Comments count | Favourite | Created at |
       | Pasta Carbonara    | Classic Italian pasta | 30           | Easy       | carbonara.jpg | dinner   | quick        | 4.5            | public     | alice  | 3              | active    | 2026-09-28 |
       | Chicken Stir Fry   | Quick Asian dish      | 25           | Medium     | stirfry.jpg   | lunch    | quick, vegan | 4.0            | private    | alice  | 1              | inactive  | 2026-09-20 |
       | Chocolate Cake     | Rich dessert          | 60           | Hard       | cake.jpg      | dessert  |              | 5.0            | private    | bob    | 0              |           | 2026-09-15 |
