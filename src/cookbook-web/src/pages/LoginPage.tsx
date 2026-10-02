import { useState, type FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth'

export function LoginPage() {
  const auth = useAuth()
  const navigate = useNavigate()
  const [userName, setUserName] = useState('alice')
  const [password, setPassword] = useState('Password1!')
  const [isRegister, setIsRegister] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  if (auth.token) return <Navigate to="/" replace />

  async function onSubmit(event: FormEvent) {
    event.preventDefault()
    setBusy(true)
    setError(null)
    try {
      if (isRegister) await auth.register(userName, password)
      else await auth.login(userName, password)
      navigate('/')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ошибка входа')
    } finally {
      setBusy(false)
    }
  }

  return (
    <main className="auth-page">
      <form className="auth-form" onSubmit={onSubmit}>
        <p className="brand">Книга рецептов</p>
        <h1>{isRegister ? 'Регистрация' : 'Вход'}</h1>
        <label>
          Имя пользователя
          <input value={userName} onChange={(e) => setUserName(e.target.value)} required minLength={3} />
        </label>
        <label>
          Пароль
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            minLength={6}
          />
        </label>
        {error && <p className="error">{error}</p>}
        <button type="submit" disabled={busy}>
          {busy ? '...' : isRegister ? 'Создать аккаунт' : 'Войти'}
        </button>
        <button type="button" className="linkish" onClick={() => setIsRegister((v) => !v)}>
          {isRegister ? 'Уже есть аккаунт? Войти' : 'Нет аккаунта? Зарегистрироваться'}
        </button>
        <Link to="/">Продолжить как гость</Link>
      </form>
    </main>
  )
}
