import { useEffect, useState, type FormEvent } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  addComment,
  deleteComment,
  getRecipe,
  photoUrl,
  setFavourite,
  setRating,
  type RecipeDetails,
} from '../api'
import { useAuth } from '../auth'
import { Layout } from '../components/Layout'

export function RecipeDetailsPage() {
  const { id = '' } = useParams()
  const { token, userName } = useAuth()
  const [recipe, setRecipe] = useState<RecipeDetails | null>(null)
  const [portions, setPortions] = useState(2)
  const [comment, setComment] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [message, setMessage] = useState<string | null>(null)

  async function reload(nextPortions = portions) {
    const data = await getRecipe(token, id, nextPortions)
    setRecipe(data)
    setPortions(data.portions)
  }

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      try {
        const data = await getRecipe(token, id)
        if (!cancelled) {
          setRecipe(data)
          setPortions(data.portions)
        }
      } catch (err) {
        if (!cancelled) setError(err instanceof Error ? err.message : 'Рецепт не найден')
      }
    })()
    return () => {
      cancelled = true
    }
  }, [id, token])

  if (error) {
    return (
      <Layout>
        <p className="error">{error}</p>
        <Link to="/">К списку</Link>
      </Layout>
    )
  }

  if (!recipe) {
    return (
      <Layout>
        <p>Загрузка...</p>
      </Layout>
    )
  }

  async function onFavourite() {
    if (!token) return
    try {
      await setFavourite(token, recipe!.id, !recipe!.isFavourite)
      await reload()
    } catch (err) {
      setMessage(err instanceof Error ? err.message : 'Ошибка')
    }
  }

  async function onRate(stars: number) {
    if (!token) return
    try {
      await setRating(token, recipe!.id, stars)
      setMessage('Оценка сохранена')
      await reload()
    } catch (err) {
      setMessage(err instanceof Error ? err.message : 'Ошибка')
    }
  }

  async function onComment(event: FormEvent) {
    event.preventDefault()
    if (!token || !comment.trim()) return
    try {
      await addComment(token, recipe!.id, comment.trim())
      setComment('')
      await reload()
    } catch (err) {
      setMessage(err instanceof Error ? err.message : 'Ошибка')
    }
  }

  async function onDeleteComment(commentId: string) {
    if (!token) return
    try {
      await deleteComment(token, recipe!.id, commentId)
      await reload()
    } catch (err) {
      setMessage(err instanceof Error ? err.message : 'Ошибка')
    }
  }

  return (
    <Layout>
      <article className="details">
        <Link to="/">← Все рецепты</Link>
        <div className="details-hero">
          <img src={photoUrl(recipe.photo)} alt="" onError={(e) => ((e.target as HTMLImageElement).style.display = 'none')} />
          <div>
            <h1>{recipe.title}</h1>
            <p>{recipe.description}</p>
            <p className="meta">
              Автор: {recipe.authorName} · {recipe.cookingTimeMinutes} мин · {recipe.category}
              {recipe.averageRating != null ? ` · ★ ${recipe.averageRating}` : ''}
            </p>
            {token && (
              <button type="button" onClick={onFavourite}>
                {recipe.isFavourite ? 'Убрать из избранного' : 'В избранное'}
              </button>
            )}
          </div>
        </div>

        <label className="portions">
          Порции
          <input
            type="number"
            min={1}
            max={20}
            value={portions}
            onChange={async (e) => {
              const value = Number(e.target.value)
              setPortions(value)
              try {
                await reload(value)
              } catch (err) {
                setMessage(err instanceof Error ? err.message : 'Ошибка')
              }
            }}
          />
        </label>

        <h2>Ингредиенты</h2>
        <ul>
          {recipe.ingredients.map((item) => (
            <li key={`${item.name}-${item.unit}`}>
              {item.name}: {item.quantity} {item.unit}
            </li>
          ))}
        </ul>

        {token && (
          <section>
            <h2>Оценка</h2>
            <div className="stars">
              {[1, 2, 3, 4, 5].map((stars) => (
                <button key={stars} type="button" onClick={() => onRate(stars)}>
                  {stars}★
                </button>
              ))}
            </div>
          </section>
        )}

        <section>
          <h2>Комментарии</h2>
          {recipe.comments.map((item) => (
            <div key={item.id} className="comment">
              <strong>{item.authorName}</strong>
              <span>{new Date(item.postedAt).toLocaleString('ru-RU')}</span>
              <p>{item.text}</p>
              {token && userName === recipe.authorName && (
                <button type="button" className="linkish" onClick={() => onDeleteComment(item.id)}>
                  Удалить
                </button>
              )}
            </div>
          ))}
          {token ? (
            <form onSubmit={onComment} className="comment-form">
              <textarea value={comment} onChange={(e) => setComment(e.target.value)} placeholder="Ваш комментарий" required />
              <button type="submit">Отправить</button>
            </form>
          ) : (
            <p>
              <Link to="/login">Войдите</Link>, чтобы оставить комментарий.
            </p>
          )}
        </section>
        {message && <p className="message">{message}</p>}
      </article>
    </Layout>
  )
}
