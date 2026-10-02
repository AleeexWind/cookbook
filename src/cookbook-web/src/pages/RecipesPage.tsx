import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getRecipes, photoUrl, type RecipeListItem } from '../api'
import { useAuth } from '../auth'
import { Layout } from '../components/Layout'

export function RecipesPage() {
  const { token } = useAuth()
  const [search, setSearch] = useState('')
  const [recipes, setRecipes] = useState<RecipeListItem[]>([])
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      try {
        const data = await getRecipes(token, search)
        if (!cancelled) setRecipes(data)
      } catch (err) {
        if (!cancelled) setError(err instanceof Error ? err.message : 'Ошибка загрузки')
      }
    })()
    return () => {
      cancelled = true
    }
  }, [token, search])

  return (
    <Layout>
      <section className="page">
        <h1>Рецепты</h1>
        <input
          className="search"
          placeholder="Поиск по названию или ингредиентам"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        {error && <p className="error">{error}</p>}
        <div className="recipe-grid">
          {recipes.map((recipe) => (
            <Link key={recipe.id} to={`/recipes/${recipe.id}`} className="recipe-tile">
              <img src={photoUrl(recipe.photo)} alt="" onError={(e) => ((e.target as HTMLImageElement).style.display = 'none')} />
              <div>
                <h2>{recipe.title}</h2>
                <p>{recipe.description}</p>
                <p className="meta">
                  {recipe.cookingTimeMinutes} мин · {difficultyRu(recipe.difficulty)}
                  {recipe.averageRating != null ? ` · ★ ${recipe.averageRating}` : ''}
                  {recipe.isFavourite ? ' · в избранном' : ''}
                </p>
              </div>
            </Link>
          ))}
        </div>
      </section>
    </Layout>
  )
}

function difficultyRu(value: string): string {
  switch (value.toLowerCase()) {
    case 'easy':
      return 'легко'
    case 'medium':
      return 'средне'
    case 'hard':
      return 'сложно'
    default:
      return value
  }
}
